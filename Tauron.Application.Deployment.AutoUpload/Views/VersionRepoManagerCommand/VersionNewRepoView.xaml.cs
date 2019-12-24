using System.Windows.Media;
using Tauron.Application.Deployment.AutoUpload.ViewModels.VersionRepoManagerCommand;
using Tauron.Application.Wpf;

namespace Tauron.Application.Deployment.AutoUpload.Views.VersionRepoManagerCommand
{
    /// <summary>
    /// Interaktionslogik für VersionNewRepoView.xaml
    /// </summary>
    [Control(typeof(VersionNewRepoViewModel))]
    public partial class VersionNewRepoView
    {
        public VersionNewRepoView(VersionNewRepoViewModel model)
            : base(model)
        {
            InitializeComponent();
            Background = Brushes.Transparent;
        }
    }
}
