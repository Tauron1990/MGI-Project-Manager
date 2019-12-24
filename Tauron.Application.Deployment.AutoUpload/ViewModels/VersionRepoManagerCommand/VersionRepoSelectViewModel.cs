using System.Linq;
using System.Threading.Tasks;
using Catel.Services;
using Scrutor;
using Tauron.Application.Deployment.AutoUpload.Models.Core;
using Tauron.Application.Deployment.AutoUpload.ViewModels.Common;
using Tauron.Application.Deployment.AutoUpload.ViewModels.Operations;
using Tauron.Application.Deployment.AutoUpload.Views.Common;
using Tauron.Application.Wpf;

namespace Tauron.Application.Deployment.AutoUpload.ViewModels.VersionRepoManagerCommand
{
    [ServiceDescriptor(typeof(VersionRepoSelectViewModel))]
    public class VersionRepoSelectViewModel : OperationViewModel<VersionRepoContext>
    {
        private readonly Settings _settings;
        private readonly IMessageService _messageService;

        public ICommonSelectorViewModel RepoSelector { get; set; } = CommonSelectorViewModel.Create();

        public VersionRepoSelectViewModel(Settings settings, IMessageService messageService)
        {
            _settings = settings;
            _messageService = messageService;
        }

        protected override Task InitializeAsync()
        {
            RepoSelector.Init(_settings.VersionRepositories.Select(vr => new VersionRepoItem(vr)), Context.Redirection == null, SelectedItemAction);

            return base.InitializeAsync();
        }

        private async Task SelectedItemAction(SelectorItemBase arg)
        {
            switch (arg)
            {
                case VersionRepoItem repoItem:
                    if (Context.Redirection != null)
                        await OnFinish();
                    break;
                case NewSelectorItem _:
                    await OnNextView<VersionNewRepoViewModel>();
                    break;
            }
        }

        [CommandTarget]
        public bool CanOnNext() => RepoSelector.CanRun();

        [CommandTarget]
        public async Task OnNext() 
            => await RepoSelector.Run();
    }
}