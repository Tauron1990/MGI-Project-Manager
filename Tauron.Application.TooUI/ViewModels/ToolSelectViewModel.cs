using System.Windows;
using Catel.MVVM;
using Scrutor;
using Tauron.Application.Deployment.AutoUpload.ViewModels;
using Tauron.Application.Deployment.ServerUI.ViewModels;
using Tauron.Application.Logging;
using Tauron.Application.ToolUI.Core;
using Tauron.Application.Wpf;

namespace Tauron.Application.ToolUI.ViewModels
{
    [ServiceDescriptor(typeof(ToolSelectViewModel))]
    [LogContextProvider(typeof(ToolSelectContext))]
    public sealed class ToolSelectViewModel : ViewModelBase, IToolWindow
    {
        private readonly IToolSwitcher _toolSwitcher;
        private readonly ISLogger<ToolSelectViewModel> _logger;

        public override string Title => "Tools";

        public WindowState WindowState => WindowState.Normal;
        public SizeToContent SizeToContent => SizeToContent.WidthAndHeight;
        public double Width => 100;
        public double Height => 100;

        public ToolSelectViewModel(IToolSwitcher toolSwitcher, ISLogger<ToolSelectViewModel> logger)
        {
            _toolSwitcher = toolSwitcher;
            _logger = logger;
        }

        [CommandTarget]
        public void OpenUpload()
        {
            _logger.Information("Open Upload Tool");
            _toolSwitcher.SwitchModel<UploadToolWindowViewModel>();
        }

        [CommandTarget]
        public void OpenDeployment()
        {
            _logger.Information("Open Deployment Tool");
            _toolSwitcher.SwitchModel<DeploymentUiViewModel>();
        }
    }
}