﻿using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Threading;
using Anotar.Serilog;
using Catel.IO;
using Catel.Services;
using LibGit2Sharp;
using Scrutor;
using Serilog.Context;
using Tauron.Application.Deployment.AutoUpload.Models;
using Tauron.Application.Deployment.AutoUpload.Models.Core;
using Tauron.Application.Deployment.AutoUpload.Models.Git;
using Tauron.Application.Deployment.AutoUpload.ViewModels.Operations;
using Tauron.Application.ToolUI.Core;

namespace Tauron.Application.Deployment.AutoUpload.ViewModels.AddCommand
{
    [ServiceDescriptor(typeof(AddSyncRepositoryViewModel))]
    public class AddSyncRepositoryViewModel : OperationViewModel<AddCommandContext>
    {
        private readonly CommonTasks _commonTasks;
        private readonly Dispatcher _dispatcher;
        private readonly AppInfo _appInfo;
        private readonly GitManager _gitManager;
        private readonly IMessageService _messageService;
        private readonly Settings _settings;

        private int _currentLine;

        private int _updateCount1 = 15;
        private int _updateCount2 = 15;

        public AddSyncRepositoryViewModel(IMessageService messageService, GitManager gitManager, Settings settings, CommonTasks commonTasks, Dispatcher dispatcher, AppInfo appInfo)
        {
            _messageService = messageService;
            _gitManager = gitManager;
            _settings = settings;
            _commonTasks = commonTasks;
            _dispatcher = dispatcher;
            _appInfo = appInfo;
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
            using(LogContext.PushProperty("Repository", Context.RegistratedRepository?.ToString()))
            {
                await Task.Delay(TimeSpan.FromSeconds(3));

                var failed = false;

                try
                {
                    LogTo.Information("Try Get User Information");
                    if (string.IsNullOrWhiteSpace(_settings.UserName) && !await _commonTasks.RequestUserInfo())
                    {
                        LogTo.Warning("No User informatione Provided");
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
                            LogTo.Information("Pulling Changes from Repository");
                            Context.RealPath = registrepo.RealPath;
                            var result = _gitManager.SyncRepo(registrepo.RealPath, ProgressHandler, TransferProgressHandler);
                            if (result.Status == MergeStatus.Conflicts)
                            {
                                LogTo.Warning("Sync Failed: {Status}--{Commit}", result.Status, result.Commit.Sha);
                                await _messageService.ShowWarningAsync("Repository Pull Fehlgeschlagen", "Fehler");
                                failed = true;
                            }
                        }
                        else
                        {
                            LogTo.Information("Clonig Repository");
                            var path = Path.Combine(_appInfo.SettingsDic, "Repos", Context.Repository.Name, Context.Branch.Name);

                            _gitManager.CloneBranch(Context.Repository.CloneUrl, Context.Branch.Name, path, ProgressHandler, TransferProgressHandler);
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
                    LogTo.Error(e, "Error on Sync Repository");
                    await _messageService.ShowErrorAsync(e);
                    failed = true;
                }

                if (failed)
                    OnCancelCommandExecute();
                else
                    await OnNextView<AddSelectProjectViewModel>();
            }
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