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

namespace Tauron.Application.Deployment.AutoUpload.ViewModels
{
    [ServiceDescriptor, MeansImplicitUse]
    public sealed class CommandViewModel : OperationViewModelBase
    {
        public Settings Settings { get; }

        public CommandViewModel(Settings settings)
        {
            Settings = settings;
            AddClieck = new TaskCommand(OnAddClieckExecute);
            BuildCommand = new TaskCommand(OnBuildCommandExecute);
            RemoveCommand = new TaskCommand(OnRemoveCommandExecute);
        }

        public TaskCommand AddClieck { get; }

        private async Task OnAddClieckExecute() 
            => await OnNextView<AddNameSelectorViewModel, AddCommandContext>(new AddCommandContext());

        public TaskCommand RemoveCommand { get; }

        private async Task OnRemoveCommandExecute()
            => await OnNextView<RemoveSelectProjectViewModel, RemoveCommandContext>(new RemoveCommandContext());

        public TaskCommand BuildCommand { get; }

        private async Task OnBuildCommandExecute()
            => await OnNextView<BuildSelectProjectViewModel, BuildOperationContext>(new BuildOperationContext(DependencyResolver.Resolve<BuildContext>()));

        
    }
}