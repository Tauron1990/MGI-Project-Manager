using System.Windows.Media;
using Tauron.Application.Deployment.AutoUpload.ViewModels.UploadCommand;
using Tauron.Application.Wpf;

namespace Tauron.Application.Deployment.AutoUpload.Views.UploadCommand
{
    /// <summary>
    ///     Interaktionslogik für UploadLastCheckView.xaml
    /// </summary>
    [Control(typeof(UploadLastCheckViewModel))]
    public partial class UploadLastCheckView
    {
        public UploadLastCheckView(UploadLastCheckViewModel model)
            : base(model)
        {
            InitializeComponent();
            Background = Brushes.Transparent;
        }
    }
}