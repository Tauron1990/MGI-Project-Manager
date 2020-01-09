using System.Windows.Media;
using Tauron.Application.Deployment.AutoUpload.ViewModels.UploadCommand;
using Tauron.Application.Wpf;

namespace Tauron.Application.Deployment.AutoUpload.Views.UploadCommand
{
    /// <summary>
    /// Interaktionslogik für UploadSelectRepoView.xaml
    /// </summary>
    [Control(typeof(UploadSelectRepoViewModel))]
    public partial class UploadSelectRepoView 
    {
        public UploadSelectRepoView(UploadSelectRepoViewModel model)
        : base(model)
        {
            InitializeComponent();
            Background = Brushes.Transparent;
        }
    }
}
