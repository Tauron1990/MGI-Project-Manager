using Scrutor;
using Tauron.Application.Deployment.AutoUpload.ViewModels.Operations;

namespace Tauron.Application.Deployment.AutoUpload.ViewModels.VersionRepoManagerCommand
{
    [ServiceDescriptor(typeof(VersionRepoSelectViewModel))]
    public class VersionRepoSelectViewModel : OperationViewModel<VersionRepoContext>
    {
        
    }
}