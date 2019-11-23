using System.Windows.Media;
using Catel;
using Tauron.Application.Deployment.AutoUpload.Core;
using Tauron.Application.Deployment.AutoUpload.ViewModels;

namespace Tauron.Application.Deployment.AutoUpload.Views
{
    /// <summary>
    /// Interaktionslogik für CommandView.xaml
    /// </summary>
    [Control(typeof(CommandViewModel))]
    public partial class CommandView
    {
        public CommandView(CommandViewModel model)
        : base(model)
        {
            InitializeComponent();
            Background = Brushes.Transparent;
        }
    }
}
