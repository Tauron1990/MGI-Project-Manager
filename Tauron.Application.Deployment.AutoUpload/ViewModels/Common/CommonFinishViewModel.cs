using System.Threading.Tasks;
using Catel.MVVM;
using Scrutor;
using Tauron.Application.Deployment.AutoUpload.ViewModels.Operations;

namespace Tauron.Application.Deployment.AutoUpload.ViewModels.Common
{
    [ServiceDescriptor(typeof(CommonFinishViewModel))]
    public class CommonFinishViewModel : OperationViewModel<FinishContext>
    {
        public string? Message { get; private set; }

        public CommonFinishViewModel() => ReturnCommand = new TaskCommand(OnReturnCommandExecute);

        protected override Task InitializeAsync()
        {
            Message = Context.Message;
            return base.InitializeAsync();
        }

        public TaskCommand ReturnCommand { get; }

        private async Task OnReturnCommandExecute() 
            => await Return();
    }
}