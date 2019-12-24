using System.Threading.Tasks;
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

        public bool IsActive { get; set; } = true;

        public string Concole { get; set; } = string.Empty;

        public VersionNewRepoViewModel(InputService inputService, Settings settings, RepositoryManager repositoryManager)
        {
            _inputService = inputService;
            _settings = settings;
            _repositoryManager = repositoryManager;
        }

        [CommandTarget]
        public bool CanOnNext() => IsActive && RepoName.Contains('/');

        [CommandTarget]
        public async Task OnNext()
        {

        }
    }
}