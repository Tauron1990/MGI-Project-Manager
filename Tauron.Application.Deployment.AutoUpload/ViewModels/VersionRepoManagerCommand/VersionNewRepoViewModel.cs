using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Catel.Collections;
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
        private readonly Settings _settings;
        private readonly RepositoryManager _repositoryManager;
        private readonly GitManager _gitManager;

        public string RepoName { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public bool IsInputActive { get; set; } = true;

        public bool IsProcessActive { get; set; }

        public FastObservableCollection<ProcesItem> Tasks { get; } = new FastObservableCollection<ProcesItem>();

        public VersionNewRepoViewModel(Settings settings, RepositoryManager repositoryManager, GitManager gitManager)
        {
            _settings = settings;
            _repositoryManager = repositoryManager;
            _gitManager = gitManager;
        }

        protected override bool CanCancelExecute() => IsInputActive;

        [CommandTarget]
        public bool CanOnNext() => IsInputActive && RepoName.Contains('/');

        [CommandTarget]
        public async Task OnNext()
        {
            IsInputActive = false;
            IsProcessActive = true;

            var currentTask = new ProcesItem("Repository Erstellen", Tasks);
            Tasks.Add(currentTask);

            Repository repo;

            try
            {
                repo = await _repositoryManager.GetRepository(RepoName);
            }
            catch (ApiException e)
            {
                if (e.StatusCode != HttpStatusCode.NotFound)
                    throw;

                repo = await _repositoryManager.CreateRepository(RepoName);
            }

            currentTask = currentTask.Next("Sync Repository");

            var path = Path.Combine(Settings.SettingsDic, "SoftwareRepos", repo.FullName);

            if (_gitManager.Exis(path))
                _gitManager.SyncRepo(path);
            else
            {
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                _gitManager.SyncBranch(repo.CloneUrl, "master", path, _ => true, _ => true);
            }

            currentTask = currentTask.Next("Repository Vorbereiten");

            var softRepo = SoftwareRepository.IsValid(path) 
                               ? await SoftwareRepository.Read(path) 
                               : await SoftwareRepository.Create(path);

            Context.VersionRepository = new VersionRepository(RepoName, path);
            await softRepo.ChangeName(RepoName.Split("/")[0], Description);
            await _settings.AddVersionRepoAndSave(Context.VersionRepository);

            currentTask = currentTask.Next("Vorgang Abschliesen");
            _gitManager.SyncRepo(path);

            currentTask.Finish();
            await OnFinish("Des Repository wurde erfolgreich angelegt");
        }
    }
}