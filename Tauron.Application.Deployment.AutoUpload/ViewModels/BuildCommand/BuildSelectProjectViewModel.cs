using Scrutor;
using Tauron.Application.Deployment.AutoUpload.ViewModels.Operations;

namespace Tauron.Application.Deployment.AutoUpload.ViewModels.BuildCommand
{
    [ServiceDescriptor(typeof(BuildSelectProjectViewModel))]
    public sealed class BuildSelectProjectViewModel : OperationViewModel<BuildOperationContext>
    {
        
    }
}