using System.Windows.Media;
using Tauron.Application.Deployment.AutoUpload.ViewModels.VersionRepoManagerCommand;
using Tauron.Application.Wpf;

namespace Tauron.Application.Deployment.AutoUpload.Views.VersionRepoManagerCommand
{
    /// <summary>
    ///     Interaktionslogik für VersionShowRepoView.xaml
    /// </summary>
    [Control(typeof(VersionShowRepoViewModel))]
    public partial class VersionShowRepoView
    {
        public VersionShowRepoView(VersionShowRepoViewModel model)
            : base(model)
        {
            InitializeComponent();
            Background = Brushes.Transparent;
        }
    }
}