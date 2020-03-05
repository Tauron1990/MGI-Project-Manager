using System.Windows.Media;
using Tauron.Application.Deployment.AutoUpload.ViewModels.AddCommand;
using Tauron.Application.Wpf;

namespace Tauron.Application.Deployment.AutoUpload.Views.AddCommand
{
    /// <summary>
    ///     Interaktionslogik für AddSelectProjectView.xaml
    /// </summary>
    [Control(typeof(AddSelectProjectViewModel))]
    public partial class AddSelectProjectView
    {
        public AddSelectProjectView(AddSelectProjectViewModel model)
            : base(model)
        {
            InitializeComponent();
            Background = Brushes.Transparent;
        }
    }
}