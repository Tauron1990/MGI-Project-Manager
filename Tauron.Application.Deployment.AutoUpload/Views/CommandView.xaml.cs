using System.Windows.Media;
using Tauron.Application.Deployment.AutoUpload.ViewModels;
using Tauron.Application.Wpf;

namespace Tauron.Application.Deployment.AutoUpload.Views
{
    /// <summary>
    ///     Interaktionslogik für CommandView.xaml
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