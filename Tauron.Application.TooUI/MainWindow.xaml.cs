using System;
using System.ComponentModel;
using System.Windows;
using Syncfusion.Licensing;
using Tauron.Application.ToolUI.Core;
using Tauron.Application.Wpf.AppCore;

namespace Tauron.Application.ToolUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : IMainWindow
    {
        public MainWindow(MainWindowViewModel model, ISkinManager manager) 
            : base(model)
        {
            model.PropertyChanged += ModelOnPropertyChanged; 

            Closed += (sender, args) => Shutdown?.Invoke(this, EventArgs.Empty);

            SyncfusionLicenseProvider.RegisterLicense(App.SyncfusionKey);

            SizeToContent = SizeToContent.Manual;
            ShowInTaskbar = true;
            ResizeMode = ResizeMode.CanResize;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            manager.Apply(this);
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
