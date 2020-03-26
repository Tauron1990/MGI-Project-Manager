using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Threading;
using Catel.IO;
using Catel.Services;
using LibGit2Sharp;
using Scrutor;
using Tauron.Application.Deployment.AutoUpload.Models;
using Tauron.Application.Deployment.AutoUpload.Models.Core;
using Tauron.Application.Deployment.AutoUpload.Models.Git;
using Tauron.Application.Deployment.AutoUpload.ViewModels.Operations;

namespace Tauron.Application.Deployment.AutoUpload.ViewModels.AddCommand
{
    [ServiceDescriptor(typeof(AddSyncRepositoryViewModel))]
    public class AddSyncRepositoryViewModel : OperationViewModel<AddCommandContext>
    {
        private readonly CommonTasks _commonTasks;
        private readonly Dispatcher _dispatcher;
        private readonly GitManager _gitManager;
        private readonly IMessageService _messageService;
        private readonly Settings _settings;

        private int _currentLine;

        private int _updateCount1 = 15;
        private int _updateCount2 = 15;

        public AddSyncRepositoryViewModel(IMessageService messageService, GitManager gitManager, Settings settings, CommonTasks commonTasks, Dispatcher dispatcher)
        {
            _messageService = messageService;
            _gitManager = gitManager;
            _settings = settings;
            _commonTasks = commonTasks;
            _dispatcher = dispatcher;
        }

        public string OutputLines { get; set; } = string.Empty;

        public bool Intermediate { get; set; } = true;

        public double ProgressValue { get; set; }

        public double ProgressMaximum { get; set; }

        public string ProgressLine { get; set; } = "Starten...";

        protected override Task InitializeAsync()
        {
            StartLoad();
            return base.InitializeAsync();
        }

        private async void StartLoad()
        {
            await Task.Delay(TimeSpan.FromSeconds(3));

            var failed = false;

            try
            {
                if (string.IsNullOrWhiteSpace(_settings.UserName) && !await _commonTasks.RequestUserInfo())
                {
                    await _messageService.ShowInformationAsync("User information nicht Verfügbar", "Fehler");
                    failed = true;
                }
                else
                {
                    var registrepo = _settings.RegistratedRepositories
                       .Where(rr => rr.RepositoryName      == Context.Repository.FullName)
                       .FirstOrDefault(rr => rr.BranchName == Context.Branch.Name);

                    if (registrepo != null)
                    {
                        Context.RealPath = registrepo.RealPath;
                        var result = _gitManager.SyncRepo(registrepo.RealPath, ProgressHandler, TransferProgressHandler);
                        if (result.Status == MergeStatus.Conflicts)
                        {
                            await _messageService.ShowWarningAsync("Repository Pull Fehlgeschlagen", "Fehler");
                            failed = true;
                        }
                    }
                    else
                    {
                        var path = Path.Combine(Settings.SettingsDic, "Repos", Context.Repository.Name, Context.Branch.Name);

                        _gitManager.SyncBranch(Context.Repository.CloneUrl, Context.Branch.Name, path, ProgressHandler, TransferProgressHandler);
                        if (string.IsNullOrWhiteSpace(path))
                        {
                            await _messageService.ShowWarningAsync("Das Klonen des Repository ist Fehlgeschlagen", "Fehler");
                            failed = true;
                        }

                        Context.RealPath = path;
                    }
                }
            }
            catch (Exception e)
            {
                await _messageService.ShowErrorAsync(e);
                failed = true;
            }

            if (failed)
                OnCancelCommandExecute();
            else
                await OnNextView<AddSelectProjectViewModel>();
        }

        private bool TransferProgressHandler(TransferProgress progress)
        {
            return TryUpdate(ref _updateCount1, () =>
                                                {
                                                    Intermediate = false;

                                                    ProgressMaximum = progress.TotalObjects;
                                                    ProgressValue = progress.ReceivedObjects;

                                                    ProgressLine = $"Objects: {progress.ReceivedObjects}/{progress.TotalObjects} -- Bytes: {progress.ReceivedBytes}";
                                                    return true;
                                                });
        }

        private bool ProgressHandler(string serverprogressoutput)
        {
            return TryUpdate(ref _updateCount2, () =>
                                                {
                                                    if (_currentLine == 10)
                                                    {
                                                        _currentLine = 0;
                                                        OutputLines = serverprogressoutput;
                                                    }
                                                    else
                                                        OutputLines = $"{OutputLines}{Environment.NewLine}{serverprogressoutput}";

                                                    _currentLine++;
                                                    return true;
                                                });
        }

        private bool TryUpdate(ref int value, Func<bool> updater)
        {
            try
            {
                if (value != 30) return true;

                value = -1;
                var result = _dispatcher.Invoke(updater, DispatcherPriority.ApplicationIdle);
                return result;
            }
            finally
            {
                value++;
            }
        }
    }
}