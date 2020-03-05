using System.Windows.Media;
using Tauron.Application.Deployment.AutoUpload.ViewModels.BuildCommand;
using Tauron.Application.Wpf;

namespace Tauron.Application.Deployment.AutoUpload.Views.BuildCommand
{
    /// <summary>
    ///     Interaktionslogik für BuildVersionIncrementView.xaml
    /// </summary>
    [Control(typeof(BuildVersionIncrementViewModel))]
    public partial class BuildVersionIncrementView
    {
        public BuildVersionIncrementView(BuildVersionIncrementViewModel model)
            : base(model)
        {
            InitializeComponent();
            Background = Brushes.Transparent;
        }
    }
}