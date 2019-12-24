using System.Threading.Tasks;
using Catel.Collections;
using Scrutor;
using Tauron.Application.Deployment.AutoUpload.Core;
using Tauron.Application.Deployment.AutoUpload.Models.Core;
using Tauron.Application.Deployment.AutoUpload.Models.Github;
using Tauron.Application.Deployment.AutoUpload.ViewModels.Operations;
using Tauron.Application.Wpf;

namespace Tauron.Application.Deployment.AutoUpload.ViewModels.VersionRepoManagerCommand
{
    [ServiceDescriptor(typeof(VersionNewRepoViewModel))]
    public class VersionNewRepoViewModel : OperationViewModel<VersionRepoContext>
    {
        private readonly InputService _inputService;
        private readonly Settings _settings;
        private readonly RepositoryManager _repositoryManager;

        public string RepoName { get; set; } = string.Empty;

        public bool IsInputActive { get; set; } = true;

        public bool IsProcessActive { get; set; }

        public FastObservableCollection<ProcesItem> Concole { get; } = new FastObservableCollection<ProcesItem>();

        public VersionNewRepoViewModel(InputService inputService, Settings settings, RepositoryManager repositoryManager)
        {
            _inputService = inputService;
            _settings = settings;
            _repositoryManager = repositoryManager;
        }

        protected override bool CanCancelExecute() => IsInputActive;

        [CommandTarget]
        public bool CanOnNext() => IsInputActive && RepoName.Contains('/');

        [CommandTarget]
        public async Task OnNext()
        {
            IsInputActive = false;
            IsProcessActive = true;
        }
    }
}