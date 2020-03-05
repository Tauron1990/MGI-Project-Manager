using System.Windows.Media;
using Tauron.Application.Deployment.AutoUpload.ViewModels.BuildCommand;
using Tauron.Application.Wpf;

namespace Tauron.Application.Deployment.AutoUpload.Views.BuildCommand
{
    /// <summary>
    ///     Interaktionslogik für BuildSelectProjectView.xaml
    /// </summary>
    [Control(typeof(BuildSelectProjectViewModel))]
    public partial class BuildSelectProjectView
    {
        public BuildSelectProjectView(BuildSelectProjectViewModel model)
            : base(model)
        {
            InitializeComponent();
            Background = Brushes.Transparent;
        }
    }
}