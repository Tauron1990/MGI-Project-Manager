using Scrutor;
using Tauron.Application.Deployment.AutoUpload.ViewModels.Operations;

namespace Tauron.Application.Deployment.AutoUpload.ViewModels.VersionRepoManagerCommand
{
    [ServiceDescriptor(typeof(VersionShowRepoViewModel))]
    public sealed class VersionShowRepoViewModel : OperationViewModel<VersionRepoContext>
    {
        
    }
}