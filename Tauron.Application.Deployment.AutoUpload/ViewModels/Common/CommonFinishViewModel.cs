using System.Threading.Tasks;
using Catel.MVVM;
using Scrutor;
using Tauron.Application.Deployment.AutoUpload.ViewModels.Operations;

namespace Tauron.Application.Deployment.AutoUpload.ViewModels.Common
{
    [ServiceDescriptor(typeof(CommonFinishViewModel))]
    public class CommonFinishViewModel : OperationViewModel<FinishContext>
    {
        public CommonFinishViewModel()
        {
            ReturnCommand = new TaskCommand(OnReturnCommandExecute);
        }

        public string? Message { get; private set; }

        public TaskCommand ReturnCommand { get; }

        protected override Task InitializeAsync()
        {
            Message = Context.Message;
            return base.InitializeAsync();
        }

        private async Task OnReturnCommandExecute()
        {
            await OnReturn();
        }
    }
}