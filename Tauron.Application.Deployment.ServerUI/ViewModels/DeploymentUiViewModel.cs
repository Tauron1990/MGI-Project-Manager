using System.Windows;
using Catel.MVVM;
using Scrutor;
using Tauron.Application.ToolUI.ViewModels;

namespace Tauron.Application.Deployment.ServerUI.ViewModels
{
    [ServiceDescriptor(typeof(DeploymentUiViewModel))]
    [LogContextProvider(typeof(DeploymentLogContext))]
    public sealed class DeploymentUiViewModel : ViewModelBase, IToolWindow
    {
        public WindowState WindowState => WindowState.Maximized;
        public SizeToContent SizeToContent => SizeToContent.Manual;
        public double Width => 1280;
        public double Height => 720;
    }
}