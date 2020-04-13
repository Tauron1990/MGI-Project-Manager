using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Syncfusion.Licensing;
using Syncfusion.SfSkinManager;
using Tauron.Application.Wpf.AppCore;

namespace Tauron.Application.ToolUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : IMainWindow
    {
        public MainWindow(MainWindowViewModel model) 
            : base(model)
        {
            model.PropertyChanged += ModelOnPropertyChanged; 

            Closed += (sender, args) => Shutdown?.Invoke(this, EventArgs.Empty);

            SyncfusionLicenseProvider.RegisterLicense(App.SyncfusionKey);

            SizeToContent = SizeToContent.Manual;
            ShowInTaskbar = true;
            ResizeMode = ResizeMode.CanResize;
            WindowStartupLocation = WindowStartupLocation.Manual;

            SfSkinManager.SetVisualStyle(this, VisualStyles.Blend);
            InitializeComponent();
            
        }

        private void ModelOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                if (e.PropertyName != nameof(MainWindowViewModel.MainContent)) return;

                var model = (MainWindowViewModel) sender;
                var content = model.MainContent;
                if (content == null) return;

                Title = content.Title;
                MinWidth = content.Width;
                MinHeight = content.Height;
                WindowState = content.WindowState;
                SizeToContent = content.SizeToContent;

                UpdateLayout();
            });
        }

        public Window Window => this;

        public event EventHandler? Shutdown;
    }
}
