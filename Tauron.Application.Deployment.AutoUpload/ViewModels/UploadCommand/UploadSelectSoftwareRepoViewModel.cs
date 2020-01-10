using Scrutor;
using Tauron.Application.Deployment.AutoUpload.ViewModels.Operations;

namespace Tauron.Application.Deployment.AutoUpload.ViewModels.UploadCommand
{
    [ServiceDescriptor(typeof(UploadSelectSoftwareRepoViewModel))]
    public sealed class UploadSelectSoftwareRepoViewModel : OperationViewModel<UploadCommandContext>
    {
        
    }
}