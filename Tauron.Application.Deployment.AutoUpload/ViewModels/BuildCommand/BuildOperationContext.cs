using Tauron.Application.Deployment.AutoUpload.Models.Build;
using Tauron.Application.Deployment.AutoUpload.ViewModels.Operations;

namespace Tauron.Application.Deployment.AutoUpload.ViewModels.BuildCommand
{
    public class BuildOperationContext : OperationContextBase
    {
        public BuildContext BuildContext { get; }

        public BuildOperationContext(BuildContext buildContext) => BuildContext = buildContext;
    }
}