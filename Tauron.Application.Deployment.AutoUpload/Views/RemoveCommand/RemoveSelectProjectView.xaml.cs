using System.Windows.Media;
using Tauron.Application.Deployment.AutoUpload.ViewModels.RemoveCommand;
using Tauron.Application.Wpf;

namespace Tauron.Application.Deployment.AutoUpload.Views.RemoveCommand
{
    /// <summary>
    ///     Interaktionslogik für RemoveSelectProjectView.xaml
    /// </summary>
    [Control(typeof(RemoveSelectProjectViewModel))]
    public partial class RemoveSelectProjectView
    {
        public RemoveSelectProjectView(RemoveSelectProjectViewModel model)
            : base(model)
        {
            InitializeComponent();
            Background = Brushes.Transparent;
        }
    }
}