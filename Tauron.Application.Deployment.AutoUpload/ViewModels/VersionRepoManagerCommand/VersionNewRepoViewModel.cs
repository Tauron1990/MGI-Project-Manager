using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Catel.Collections;
using Catel.Services;
using Octokit;
using Scrutor;
using Tauron.Application.Deployment.AutoUpload.Models.Core;
using Tauron.Application.Deployment.AutoUpload.Models.Git;
using Tauron.Application.Deployment.AutoUpload.Models.Github;
using Tauron.Application.Deployment.AutoUpload.ViewModels.Operations;
using Tauron.Application.SoftwareRepo;
using Tauron.Application.Wpf;

namespace Tauron.Application.Deployment.AutoUpload.ViewModels.VersionRepoManagerCommand
{
    [ServiceDescriptor(typeof(VersionNewRepoViewModel))]
    public class VersionNewRepoViewModel : OperationViewModel<VersionRepoContext>
    {
        private readonly GitManager _gitManager;
        private readonly IMessageService _messageService;
        private readonly RepositoryManager _repositoryManager;
        private readonly Settings _settings;

        public VersionNewRepoViewModel(Settings settings, RepositoryManager repositoryManager, GitManager gitManager, IMessageService messageService)
        {
            _settings = settings;
            _repositoryManager = repositoryManager;
            _gitManager = gitManager;
            _messageService = messageService;
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
            try
            {
                IsInputActive = false;
                IsProcessActive = true;
                var isExistend = false;

                var currentTask = new ProcesItem("Repository Erstellen", Tasks);
                Tasks.Add(currentTask);

                Repository repo;

                try
                {
                    repo = await _repositoryManager.GetRepository(RepoName);
                    isExistend = true;
                }
                catch (ApiException e)
                {
                    if (e.StatusCode != HttpStatusCode.NotFound)
                        throw;

                    repo = await _repositoryManager.CreateRepository(RepoName);
                }

                var path = Path.Combine(Settings.SettingsDic, "SoftwareRepos", repo.FullName);

                currentTask = currentTask.Next("Sync Repository");

                if (isExistend)
                    _gitManager.SyncRepo(path);
                else
                    _gitManager.CreateRepository(path, repo.CloneUrl);

                currentTask = currentTask.Next("Repository Vorbereiten");

                var softRepo = SoftwareRepository.IsValid(path)
                    ? await SoftwareRepository.Read(path)
                    : await SoftwareRepository.Create(path);

                Context.VersionRepository = new VersionRepository(RepoName, path, repo.Id);
                await softRepo.ChangeName(RepoName.Split("/")[0], Description);
                await _settings.AddVersionRepoAndSave(Context.VersionRepository);

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