using System.Linq;
using System.Threading.Tasks;
using Catel.Services;
using Scrutor;
using Tauron.Application.Deployment.AutoUpload.Models.Core;
using Tauron.Application.Deployment.AutoUpload.ViewModels.Common;
using Tauron.Application.Deployment.AutoUpload.ViewModels.Operations;
using Tauron.Application.Wpf;

namespace Tauron.Application.Deployment.AutoUpload.ViewModels.VersionRepoManagerCommand
{
    [ServiceDescriptor(typeof(VersionRepoSelectViewModel))]
    public class VersionRepoSelectViewModel : OperationViewModel<VersionRepoContext>
    {
        private readonly IMessageService _messageService;
        private readonly Settings _settings;

        public VersionRepoSelectViewModel(Settings settings, IMessageService messageService)
        {
            _settings = settings;
            _messageService = messageService;
        }

        public ICommonSelectorViewModel RepoSelector { get; set; } = CommonSelectorViewModel.Create();

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
                    Context.VersionRepository = repoItem.VersionRepository;
                    if (Context.Redirection != null)
                        await OnFinish();
                    else
                        await OnNextView<VersionShowRepoViewModel>();
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
        {
            await RepoSelector.Run();
        }
    }
}