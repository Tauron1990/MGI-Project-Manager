using System.Threading.Tasks;
using Catel.MVVM;
using JetBrains.Annotations;
using Scrutor;
using Tauron.Application.Deployment.AutoUpload.Models.Core;
using Tauron.Application.Deployment.AutoUpload.ViewModels.AddCommand;
using Tauron.Application.Deployment.AutoUpload.ViewModels.Operations;

namespace Tauron.Application.Deployment.AutoUpload.ViewModels
{
    [ServiceDescriptor, MeansImplicitUse]
    public sealed class CommandViewModel : OperationViewModelBase
    {
        public Settings Settings { get; }

        public CommandViewModel(Settings settings)
        {
            Settings = settings;
            AddClieck = new Command(OnAddClieckExecute);
        }

        public Command AddClieck { get; }

        private void OnAddClieckExecute()
        {
            OnNextView(typeof(AddNameSelectorViewModel), new AddCommandContext());
        }
    }
}