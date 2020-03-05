using System.Windows.Media;
using Tauron.Application.Deployment.AutoUpload.ViewModels.UploadCommand;
using Tauron.Application.Wpf;

namespace Tauron.Application.Deployment.AutoUpload.Views.UploadCommand
{
    /// <summary>
    ///     Interaktionslogik für UploadSelectSoftwareRepoView.xaml
    /// </summary>
    [Control(typeof(UploadSelectSoftwareRepoViewModel))]
    public partial class UploadSelectSoftwareRepoView
    {
        public UploadSelectSoftwareRepoView(UploadSelectSoftwareRepoViewModel model)
            : base(model)
        {
            InitializeComponent();
            Background = Brushes.Transparent;
        }
    }
}