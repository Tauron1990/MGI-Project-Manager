using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Catel.Collections;
using Catel.Services;
using Octokit;
using Scrutor;
using Serilog.Context;
using Tauron.Application.Deployment.AutoUpload.Models.Core;
using Tauron.Application.Deployment.AutoUpload.Models.Git;
using Tauron.Application.Deployment.AutoUpload.Models.Github;
using Tauron.Application.Deployment.AutoUpload.ViewModels.Operations;
using Tauron.Application.Logging;
using Tauron.Application.SoftwareRepo;
using Tauron.Application.Wpf;

namespace Tauron.Application.Deployment.AutoUpload.ViewModels.VersionRepoManagerCommand
{
    [ServiceDescriptor(typeof(VersionNewRepoViewModel))]
    public class VersionNewRepoViewModel : OperationViewModel<VersionRepoContext>
    {
        private readonly GitManager _gitManager;
        private readonly IMessageService _messageService;
        private readonly ISLogger<VersionNewRepoViewModel> _logger;
        private readonly IRepoFactory _repoFactory;
        private readonly RepositoryManager _repositoryManager;
        private readonly Settings _settings;

        public VersionNewRepoViewModel(Settings settings, RepositoryManager repositoryManager, GitManager gitManager, IMessageService messageService, ISLogger<VersionNewRepoViewModel> logger,
            IRepoFactory repoFactory)
        {
            _settings = settings;
            _repositoryManager = repositoryManager;
            _gitManager = gitManager;
            _messageService = messageService;
            _logger = logger;
            _repoFactory = repoFactory;
        }

        public string RepoName { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public bool IsInputActive { get; set; } = true;

        public bool IsProcessActive { get; set; }

        public FastObservableCollection<ProcesItem> Tasks { get; } = new FastObservableCollection<ProcesItem>();

        protected override bool CanCancelExecute() => IsInputActive;

        [CommandTarget]
        public bool CanOnNext() => IsInputActive && RepoName.Contains('/');

        [CommandTarget]
        public async Task OnNext()
        {
            using (LogContext.PushProperty("RepositoryName", RepoName))
            {
                try
                {
                    _logger.Information("Begin Creating New Repository");

                    IsInputActive = false;
                    IsProcessActive = true;
                    var isExistend = false;

                    var currentTask = new ProcesItem("Repository Erstellen", Tasks);
                    Tasks.Add(currentTask);

                    Repository repo;

                    try
                    {
                        _logger.Information("Try get Repository");

                        repo = await _repositoryManager.GetRepository(RepoName);
                        isExistend = true;
                    }
                    catch (ApiException e)
                    {
                        _logger.Information("Repository Not Found");

                        if (e.StatusCode != HttpStatusCode.NotFound)
                        {
                            _logger.Error(e, "Github Api Error");
                            throw;
                        }

                        _logger.Information("Create Repository");
                        repo = await _repositoryManager.CreateRepository(RepoName);
                    }

                    var path = Path.Combine(Settings.SettingsDic, "SoftwareRepos", repo.FullName);

                    currentTask = currentTask.Next("Sync Repository");

                    _logger.Information("Syncronize Repository to Local Directory");
                    if (isExistend)
                        _gitManager.SyncRepo(path);
                    else
                        _gitManager.CreateRepository(path, repo.CloneUrl);

                    currentTask = currentTask.Next("Repository Vorbereiten");

                    _logger.Information("Read Software Repository");
                    var softRepo = _repoFactory.IsValid(path)
                        ? await _repoFactory.Read(path)
                        : await _repoFactory.Create(path);

                    _logger.Information("Update Repository List");
                    Context.VersionRepository = new VersionRepository(RepoName, path, repo.Id);
                    await softRepo.ChangeName(RepoName.Split("/")[0], Description);
                    await _settings.AddVersionRepoAndSave(Context.VersionRepository);

                    _logger.Information("Commit Repository Changes");
                    currentTask = currentTask.Next("Vorgang Abschliesen");
                    _gitManager.CommitRepo(Context.VersionRepository);

                    currentTask.Finish();
                    await OnFinish("Des Repository wurde erfolgreich angelegt");
                }
                catch (Exception e)
                {
                    await _messageService.ShowErrorAsync(e);
                    await OnReturn();
                }
            }
        }
    }
}