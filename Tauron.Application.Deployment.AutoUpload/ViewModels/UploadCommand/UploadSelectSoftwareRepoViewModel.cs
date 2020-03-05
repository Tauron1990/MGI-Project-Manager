using System.Linq;
using System.Threading.Tasks;
using Scrutor;
using Tauron.Application.Deployment.AutoUpload.Models.Core;
using Tauron.Application.Deployment.AutoUpload.Models.Github;
using Tauron.Application.Deployment.AutoUpload.ViewModels.Common;
using Tauron.Application.Deployment.AutoUpload.ViewModels.Operations;
using Tauron.Application.Deployment.AutoUpload.ViewModels.VersionRepoManagerCommand;
using Tauron.Application.Wpf;

namespace Tauron.Application.Deployment.AutoUpload.ViewModels.UploadCommand
{
    [ServiceDescriptor(typeof(UploadSelectSoftwareRepoViewModel))]
    public sealed class UploadSelectSoftwareRepoViewModel : OperationViewModel<UploadCommandContext>
    {
        private readonly Settings _settings;

        public UploadSelectSoftwareRepoViewModel(Settings settings)
        {
            _settings = settings;
        }

        public ICommonSelectorViewModel RepoSelector { get; } = CommonSelectorViewModel.Create();

        protected override async Task InitializeAsync()
        {
            RepoSelector.Init(_settings.VersionRepositories.Select(s => new VersionRepo(s)), true, SelectedItemAction);

            await base.InitializeAsync();
        }

        [CommandTarget]
        public async Task OnNext()
        {
            await RepoSelector.Run();
        }

        [CommandTarget]
        public bool CanOnNext()
        {
            return RepoSelector.CanRun();
        }

        private async Task SelectedItemAction(SelectorItemBase arg)
        {
            if (arg.ItemType == ItemType.New)
            {
                await OnNextView<VersionNewRepoViewModel, VersionRepoContext>(new VersionRepoContext(), CreateRedirection<UploadLastCheckViewModel>());
            }
            else
            {
                Context.VersionRepository = ((VersionRepo) arg).Repository;
                await OnNextView<UploadLastCheckViewModel>();
            }
        }

        private sealed class VersionRepo : SelectorItemBase
        {
            public VersionRepo(VersionRepository repository)
            {
                Repository = repository;
            }

            public VersionRepository Repository { get; }

            public override string Name => Repository.Name;
            public override ItemType ItemType => ItemType.Item;
        }
    }
}