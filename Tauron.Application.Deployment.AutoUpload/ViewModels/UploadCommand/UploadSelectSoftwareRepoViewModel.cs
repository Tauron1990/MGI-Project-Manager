using System.Linq;
using System.Threading.Tasks;
using Catel.Services;
using Scrutor;
using Tauron.Application.Deployment.AutoUpload.Models.Core;
using Tauron.Application.Deployment.AutoUpload.Models.Github;
using Tauron.Application.Deployment.AutoUpload.ViewModels.Common;
using Tauron.Application.Deployment.AutoUpload.ViewModels.Operations;

namespace Tauron.Application.Deployment.AutoUpload.ViewModels.UploadCommand
{
    [ServiceDescriptor(typeof(UploadSelectSoftwareRepoViewModel))]
    public sealed class UploadSelectSoftwareRepoViewModel : OperationViewModel<UploadCommandContext>
    {
        private sealed class VersionRepo : SelectorItemBase
        {
            public VersionRepository Repository { get; }

            public VersionRepo(VersionRepository repository) 
                => Repository = repository;

            public override string Name => Repository.Name;
            public override ItemType ItemType => ItemType.Item;
        }

        private readonly Settings _settings;

        public ICommonSelectorViewModel RepoSelector { get; } = CommonSelectorViewModel.Create();

        public UploadSelectSoftwareRepoViewModel(Settings settings) 
            => _settings = settings;

        protected override async Task InitializeAsync()
        {
            RepoSelector.Init(_settings.VersionRepositories.Select(s => new VersionRepo(s)), true, SelectedItemAction);

            await base.InitializeAsync();
        }

        private Task SelectedItemAction(SelectorItemBase arg)
        {
            
        }
    }
}