using System.Windows.Media;
using Tauron.Application.Deployment.AutoUpload.ViewModels.BuildCommand;
using Tauron.Application.Wpf;

namespace Tauron.Application.Deployment.AutoUpload.Views.BuildCommand
{
    /// <summary>
    ///     Interaktionslogik für BuildErrorView.xaml
    /// </summary>
    [Control(typeof(BuildErrorViewModel))]
    public partial class BuildErrorView
    {
        public BuildErrorView(BuildErrorViewModel model)
            : base(model)
        {
            InitializeComponent();
            Background = Brushes.Transparent;
        }
    }
}