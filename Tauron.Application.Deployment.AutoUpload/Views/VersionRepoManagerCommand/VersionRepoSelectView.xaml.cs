using System.Windows.Media;
using Tauron.Application.Deployment.AutoUpload.ViewModels.VersionRepoManagerCommand;
using Tauron.Application.Wpf;

namespace Tauron.Application.Deployment.AutoUpload.Views.VersionRepoManagerCommand
{
    /// <summary>
    ///     Interaktionslogik für VersionRepoSelectView.xaml
    /// </summary>
    [Control(typeof(VersionRepoSelectViewModel))]
    public partial class VersionRepoSelectView
    {
        public VersionRepoSelectView(VersionRepoSelectViewModel model)
            : base(model)
        {
            InitializeComponent();

            Background = Brushes.Transparent;
        }
    }
}