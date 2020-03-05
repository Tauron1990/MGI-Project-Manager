using System.Windows.Media;
using Tauron.Application.Deployment.AutoUpload.ViewModels.BuildCommand;
using Tauron.Application.Wpf;

namespace Tauron.Application.Deployment.AutoUpload.Views.BuildCommand
{
    /// <summary>
    ///     Interaktionslogik für BuildBuildView.xaml
    /// </summary>
    [Control(typeof(BuildBuildViewModel))]
    public partial class BuildBuildView
    {
        public BuildBuildView(BuildBuildViewModel model)
            : base(model)
        {
            InitializeComponent();
            Background = Brushes.Transparent;
        }
    }
}