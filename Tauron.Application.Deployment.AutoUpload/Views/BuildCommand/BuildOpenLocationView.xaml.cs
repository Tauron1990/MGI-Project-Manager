using System.Windows.Media;
using Tauron.Application.Deployment.AutoUpload.ViewModels.BuildCommand;
using Tauron.Application.Wpf;

namespace Tauron.Application.Deployment.AutoUpload.Views.BuildCommand
{
    /// <summary>
    ///     Interaktionslogik für BuildOpenLocationView.xaml
    /// </summary>
    [Control(typeof(BuildOpenLocationViewModel))]
    public partial class BuildOpenLocationView
    {
        public BuildOpenLocationView(BuildOpenLocationViewModel model)
            : base(model)
        {
            InitializeComponent();
            Background = Brushes.Transparent;
        }
    }
}