using Tauron.Application.Deployment.AutoUpload.ViewModels.UploadCommand;
using Tauron.Application.Wpf;

namespace Tauron.Application.Deployment.AutoUpload.Views.UploadCommand
{
    /// <summary>
    /// Interaktionslogik für UploadCreatePackageView.xaml
    /// </summary>
    [Control(typeof(UploadCreatePackageViewModel))]
    public partial class UploadCreatePackageView
    {
        public UploadCreatePackageView(UploadCreatePackageViewModel model)
            : base(model)
        {
            InitializeComponent();
        }
    }
}
