using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Scrutor;
using Tauron.Application.Deployment.AutoUpload.Models.Core;
using Tauron.Application.Deployment.AutoUpload.Models.Github;
using Tauron.Application.Deployment.AutoUpload.ViewModels.AddCommand;
using Tauron.Application.Deployment.AutoUpload.ViewModels.Common;
using Tauron.Application.Deployment.AutoUpload.ViewModels.Operations;
using Tauron.Application.Wpf;

namespace Tauron.Application.Deployment.AutoUpload.ViewModels.UploadCommand
{
    [ServiceDescriptor(typeof(UploadSelectRepoViewModel))]
    public class UploadSelectRepoViewModel : OperationViewModel<UploadCommandContext>
    {
        private readonly Settings _settings;

        public UploadSelectRepoViewModel(Settings settings) => _settings = settings;

        public ICommonSelectorViewModel RepoSelector { get; } = CommonSelectorViewModel.Create();

        protected override async Task InitializeAsync()
        {
            RepoSelector.Init(_settings.RegistratedRepositories.Select(rr => new RepoItem(rr)), true, SelectedItemAction);

            await base.InitializeAsync();
        }

        [CommandTarget]
        public async Task OnNext()
        {
            await RepoSelector.Run();
        }

        [CommandTarget]
        public bool CanOnNext() => RepoSelector.CanRun();

        private async Task SelectedItemAction(SelectorItemBase arg)
        {
            if (arg.ItemType == ItemType.New)
                await OnNextView<AddNameSelectorViewModel, AddCommandContext>(new AddCommandContext(), CreateRedirection<UploadSelectSoftwareRepoViewModel>());
            else
            {
                Context.Repository = ((RepoItem) arg).Repository;
                await OnNextView<UploadSelectSoftwareRepoViewModel>();
            }
        }

        private class RepoItem : SelectorItemBase
        {
            public RepoItem(RegistratedRepository repo) => Repository = repo;

            public RegistratedRepository Repository { get; }

            public override string Name => Path.GetFileName(Repository.ProjectName);
            public override ItemType ItemType => ItemType.Item;
        }
    }
}