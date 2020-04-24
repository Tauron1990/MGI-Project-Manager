using System.Windows.Media;
using Tauron.Application.Deployment.ServerUI.ViewModels;
using Tauron.Application.Wpf;

namespace Tauron.Application.Deployment.ServerUI.Views
{
    /// <summary>
    /// Interaktionslogik für DeploymentUiView.xaml
    /// </summary>
    [Control(typeof(DeploymentUiViewModel))]
    public partial class DeploymentUiView
    {
        public DeploymentUiView(DeploymentUiViewModel model)
            : base(model)
        {
            InitializeComponent();
            Background = Brushes.Transparent;
        }
    }
}
