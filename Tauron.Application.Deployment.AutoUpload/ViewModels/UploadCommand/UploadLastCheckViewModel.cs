using Scrutor;
using Tauron.Application.Deployment.AutoUpload.ViewModels.Operations;

namespace Tauron.Application.Deployment.AutoUpload.ViewModels.UploadCommand
{
    [ServiceDescriptor(typeof(UploadLastCheckViewModel))]
    public class UploadLastCheckViewModel : OperationViewModel<UploadCommandContext>
    {
        
    }
}