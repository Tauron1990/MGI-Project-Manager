using System.Windows;
using Catel.Windows;
using Syncfusion.Licensing;
using Syncfusion.SfSkinManager;
using Tauron.Application.Deployment.AutoUpload.ViewModels;
using Tauron.Application.Wpf;

namespace Tauron.Application.Deployment.AutoUpload
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    [Control(typeof(MainWindowViewModel))]
    public partial class MainWindow
    {
        public MainWindow(MainWindowViewModel model)
            : base(model, DataWindowMode.Custom)
        {
            SyncfusionLicenseProvider.RegisterLicense(App.SyncfusionKey);

            SizeToContent = SizeToContent.Manual;
            ShowInTaskbar = true;
            ResizeMode = ResizeMode.CanResize;
            WindowStartupLocation = WindowStartupLocation.Manual;

            SfSkinManager.SetVisualStyle(this, VisualStyles.Blend);
            InitializeComponent();
        }
    }
}