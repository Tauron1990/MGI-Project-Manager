using System.Threading.Tasks;
using Catel.IoC;
using Catel.MVVM;
using JetBrains.Annotations;
using Scrutor;
using Tauron.Application.Deployment.AutoUpload.Models.Build;
using Tauron.Application.Deployment.AutoUpload.Models.Core;
using Tauron.Application.Deployment.AutoUpload.ViewModels.AddCommand;
using Tauron.Application.Deployment.AutoUpload.ViewModels.BuildCommand;
using Tauron.Application.Deployment.AutoUpload.ViewModels.Operations;
using Tauron.Application.Deployment.AutoUpload.ViewModels.RemoveCommand;
using Tauron.Application.Deployment.AutoUpload.ViewModels.UploadCommand;
using Tauron.Application.Deployment.AutoUpload.ViewModels.VersionRepoManagerCommand;
using Tauron.Application.Wpf;

namespace Tauron.Application.Deployment.AutoUpload.ViewModels
{
    [ServiceDescriptor]
    [MeansImplicitUse]
    public sealed class CommandViewModel : OperationViewModelBase
    {
        public CommandViewModel(Settings settings)
        {
            Settings = settings;
            AddClieck = new TaskCommand(OnAddClieckExecute);
            BuildCommand = new TaskCommand(OnBuildCommandExecute);
            RemoveCommand = new TaskCommand(OnRemoveCommandExecute);
        }

        public Settings Settings { get; }

        public TaskCommand AddClieck { get; }

        public TaskCommand RemoveCommand { get; }

        public TaskCommand BuildCommand { get; }

        private async Task OnAddClieckExecute()
        {
            await OnNextView<AddNameSelectorViewModel, AddCommandContext>(new AddCommandContext());
        }

        private async Task OnRemoveCommandExecute()
        {
            await OnNextView<RemoveSelectProjectViewModel, RemoveCommandContext>(new RemoveCommandContext());
        }

        private async Task OnBuildCommandExecute()
        {
            await OnNextView<BuildSelectProjectViewModel, BuildOperationContext>(new BuildOperationContext(DependencyResolver.Resolve<BuildContext>()));
        }

        [CommandTarget]
        public async Task OnVersionRepoManager()
        {
            await OnNextView<VersionRepoSelectViewModel, VersionRepoContext>(new VersionRepoContext());
        }

        [CommandTarget]
        public async Task OnUploadFile()
        {
            await OnNextView<UploadSelectRepoViewModel, UploadCommandContext>(new UploadCommandContext());
        }
    }
}