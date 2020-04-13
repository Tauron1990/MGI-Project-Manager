using Tauron.Application.Deployment.AutoUpload.ViewModels;
using Tauron.Application.Wpf;

namespace Tauron.Application.Deployment.AutoUpload
{
    /// <summary>
    ///     Interaction logic for UploadToolWindow.xaml
    /// </summary>
    [Control(typeof(UploadToolWindowViewModel))]
    public partial class UploadToolWindow
    {
        public UploadToolWindow(UploadToolWindowViewModel model)
            : base(model) =>
            InitializeComponent();
    }
}