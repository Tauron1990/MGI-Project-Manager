using Tauron.Application.ToolUI.ViewModels;

namespace Tauron.Application.Deployment.ServerUI
{
    public sealed class DeploymentLogContext : LogContextBase
    {
        protected override object ContextData() 
            => "Deployment-Tool";
    }
}