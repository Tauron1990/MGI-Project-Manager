using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;
using Catel.Services;
using Catel.Threading;
using Scrutor;
using Serilog.Context;
using Tauron.Application.Deployment.AutoUpload.Models.Core;
using Tauron.Application.Deployment.AutoUpload.Models.Git;
using Tauron.Application.Deployment.AutoUpload.Models.Github;
using Tauron.Application.Deployment.AutoUpload.ViewModels.BuildCommand;
using Tauron.Application.Deployment.AutoUpload.ViewModels.Operations;
using Tauron.Application.Logging;
using Tauron.Application.SoftwareRepo;
using Tauron.Application.ToolUI.Core;
using Tauron.Application.Wpf;

namespace Tauron.Application.Deployment.AutoUpload.ViewModels.UploadCommand
{
    [ServiceDescriptor(typeof(UploadCreatePackageViewModel))]
    public class UploadCreatePackageViewModel : OperationViewModel<UploadCommandContext>, IDisposable
    {
        private readonly CancellationTokenSource _cancel = new CancellationTokenSource();
        private readonly GitManager _gitManager;

        private readonly IMessageService _messageService;

        private readonly ProcessItem _operation;
        private readonly RepositoryManager _repositoryManager;
        private readonly IRepoFactory _repoFactory;
        private readonly ISLogger<UploadCreatePackageViewModel> _logger;
        private readonly AppInfo _appInfo;
        private readonly CancellationToken _token;

        public UploadCreatePackageViewModel(IMessageService messageService, GitManager gitManager, RepositoryManager repositoryManager, IRepoFactory repoFactory, 
            ISLogger<UploadCreatePackageViewModel> logger, AppInfo appInfo)
        {
            _messageService = messageService;
            _gitManager = gitManager;
            _repositoryManager = repositoryManager;
            _repoFactory = repoFactory;
            _logger = logger;
            _appInfo = appInfo;
            _token = _cancel.Token;
            _operation = new ProcessItem(AddConsole, ThrowCanceled, () => IsFailed, s => CancelLabel = s);
        }

        public string Console { get; set; } = string.Empty;

        private bool IsFailed { get; set; }

        public string CancelLabel { get; set; } = "Abbrechen";

        public void Dispose()
        {
            _cancel.Dispose();
        }

        [CommandTarget]
        public async Task CancelOp()
        {
            if (IsFailed)
                await OnReturn();
            else
                _cancel.Cancel();
        }

        [CommandTarget]
        public bool CanCancelOp() => !_cancel.IsCancellationRequested;

        protected override Task InitializeAsync()
        {
            Task.Run(StartOperation, _token);
            return base.InitializeAsync();
        }

        private async void StartOperation()
        {
            _logger.Information("Checking Build for Packaging");
            AddConsole("Überprüfe Build...");

            if (Context.Output == null)
            {
                _logger.Information("No Build Found");
                await _messageService.ShowErrorAsync("Kein Build Gefunden");
                await OnReturn();
                return;
            }

            await Context.Output.Do(Run, Faild);
        }

        private async Task Faild(BuildFailed arg)
        {
            _logger.Information("Build Failed {ErrorCode}", arg.Result);
            Console = $"Build Fehlerhaft:{Environment.NewLine}{arg.Console}{Environment.NewLine}Prozess Benende mit: {arg.Result} Code -- Fehlerzahl: {arg.ErrorCount}";
            IsFailed = true;
            CancelLabel = "Zurück";
            await _messageService.ShowErrorAsync("Fehler Beim erstllen");
        }

        private async Task Run(string output, Version version)
        {
            using (LogContext.PushProperty("Repository", Context.VersionRepository?.Name))
            {
                var op = _operation;

                var versionRepo = Context.VersionRepository;
                if (versionRepo == null)
                {
                    _logger.Information("No Repository Found");
                    AddConsole("Kein Versions Repository gefunden");
                    IsFailed = true;
                    CancelLabel = "Zurück";
                    return;
                }

                var versionUserName = Context.VersionRepository?.Name.Split("/")[0] ?? string.Empty;
                var packagePath = string.Empty;
                var name = string.Empty;
                // ReSharper disable once RedundantAssignment
                var asset = default((string, int));

                try
                {
                    await op.Next("Zip wird erstellt...",
                        async () =>
                        {
                            _logger.Information("Creating Package Zip File");
                            packagePath = Path.Combine(_appInfo.SettingsDic, "package.zip");
                            name = Path.GetFileNameWithoutExtension(Context.Repository?.ProjectName ?? string.Empty);
                            if (string.IsNullOrWhiteSpace(name))
                            {
                                _logger.Warning("Application not Found: {App}", name);
                                IsFailed = true;
                                await _messageService.ShowErrorAsync("Name der Anwendung nicht gefunden");
                                return null;
                            }

                            if (File.Exists(packagePath))
                                File.Delete(packagePath);

                            _logger.Information("Compressing Zip File");
                            ZipFile.CreateFromDirectory(output, packagePath, CompressionLevel.Optimal, false);
                            return () =>
                            {
                                if (File.Exists(packagePath))
                                    File.Delete(packagePath);
                            };
                        });

                    await op.Next("Sync Version Repository...",
                        () =>
                        {
                            _logger.Information("Syncronize Software Repository");
                            _gitManager.SyncRepo(versionRepo.RealPath);
                            return Task.FromResult<Action?>(null);
                        });

                    await op.Next("Release Hochladen...",
                        async () =>
                        {
                            var assetName = $"{name}_{version}.zip";
                            _logger.Information("Upload Application Package File: {AssetName}", assetName);
                            asset = await _repositoryManager.UploadAsset(versionRepo.Id, packagePath, assetName, versionUserName);
                            return () => _repositoryManager.DeleteRelease(versionRepo.Id, asset.Item2, versionUserName)
                               .WaitAndUnwrapException(_token);
                        });

                    await op.Next("Aktualisiere Software Repository...",
                        async () =>
                        {
                            _logger.Information("Update Software Repository");
                            var url = asset.Item1;
                            var vrepo = Context.VersionRepository;
                            var srepo = Context.Repository;

                            if (url == null || vrepo == null || srepo == null)
                            {
                                _logger.Warning("No download url found for {Name}", name);
                                IsFailed = true;
                                await _messageService.ShowErrorAsync("Kein'e Download Url/Repository Gefunden");
                                return null;
                            }

                            var repo = await _repoFactory.Read(versionRepo.RealPath);
                            var backup = repo.CreateBackup();

                            var id = repo.Get(name);

                            if (id != -1)
                            {
                                _logger.Information("Update Application: {Name}", name);
                                repo.UpdateApplication(id, version, url);
                            }
                            else
                            {
                                _logger.Information("Add Application: {Name}", name);
                                repo.AddApplication(name, DateTime.Now.Ticks, url, version, srepo.RepositoryName, srepo.BranchName);
                            }

                            _logger.Information("Save Repository");
                            await repo.Save();
                            _gitManager.CommitRepo(vrepo);

                            return () => repo.Revert(backup);
                        });

                    await op.Next("Aufräumen...", async () =>
                    {
                        try
                        {
                            _logger.Information("Clean Up Build Files");
                            File.Delete(packagePath);
                            Directory.Delete(output, true);
                        }
                        catch (IOException e)
                        {
                            _logger.Warning(e, "Error on Clean Up");
                        }

                        await Task.Delay(2000, _token);

                        return null;
                    });

                    await OnFinish("Software Erfolgreich Aktualisiert");
                }
                catch (Exception e)
                {
                    _logger.Error(e, "Error on Build Application Package");
                    if (!(e is OperationCanceledException))
                        await _messageService.ShowErrorAsync(e);

                    try
                    {
                        _operation.Revert();
                    }
                    catch (Exception exception)
                    {
                        await _messageService.ShowErrorAsync(exception);
                    }
                }
            }
        }

        private void ThrowCanceled()
        {
            _token.ThrowIfCancellationRequested();
        }

        private void AddConsole(string value)
        {
            Console = $"{Console}{Environment.NewLine}{value}";
        }

        private class ProcessItem
        {
            private readonly Action _cancel;
            private readonly Action<string> _cancelLabel;
            private readonly Func<bool> _failed;
            private readonly Action<string> _label;
            private readonly List<Action> _revert = new List<Action>();

            public ProcessItem(Action<string> label, Action cancel, Func<bool> failed, Action<string> cancelLabel)
            {
                _label = label;
                _cancel = cancel;
                _failed = failed;
                _cancelLabel = cancelLabel;
            }

            public void Revert()
            {
                _label("Zurücksetzen...");

                foreach (var action in _revert)
                    action();
            }

            public async Task Next(string label, Func<Task<Action?>> action)
            {
                _label(label);
                _cancel();

                var revertaction = await action();
                if (revertaction == null) return;
                _revert.Add(revertaction);

                if (_failed())
                {
                    _cancelLabel("Zurück");
                    throw new OperationCanceledException();
                }
            }
        }
    }
}