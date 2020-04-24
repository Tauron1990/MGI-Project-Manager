using System.Threading.Tasks;
using System.Windows;
using Anotar.Serilog;
using Catel.Messaging;
using Catel.MVVM;
using Scrutor;
using Tauron.Application.ToolUI.Core;
using Tauron.Application.ToolUI.ViewModels;
using Tauron.Application.Wpf;

namespace Tauron.Application.Deployment.ServerUI.ViewModels
{
    [ServiceDescriptor(typeof(DeploymentUiViewModel))]
    [LogContextProvider(typeof(DeploymentLogContext))]
    public sealed class DeploymentUiViewModel : ScopeProvider, IToolWindow
    {
        private readonly IToolSwitcher _toolSwitcher;
        private readonly IMessageMediator _mediator;

        public WindowState WindowState => WindowState.Maximized;
        public SizeToContent SizeToContent => SizeToContent.Manual;
        public double Width => 1280;
        public double Height => 720;

        public ViewModelBase? CurrentView { get; set; }

        public DeploymentUiViewModel(IToolSwitcher toolSwitcher)
        {
            _toolSwitcher = toolSwitcher;
            Title = "Deployment Server Tool";
        }

        [EventTarget]
        public void CloseTool() 
            => _toolSwitcher.Return();

    }
}