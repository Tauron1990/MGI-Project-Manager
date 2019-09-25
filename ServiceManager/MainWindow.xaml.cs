using System;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using MessageBox = System.Windows.MessageBox;

namespace ServiceManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private IServiceProvider _serviceProvider;
        private MainWindowsModel _model;

        public MainWindow()
        {
            InitializeComponent();

            Icon = BitmapFrame.Create(File.Open("AppIcon.ico", FileMode.Open));
        }

        private async void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            _serviceProvider = App.CreateServiceCollection();
            _model = _serviceProvider.GetRequiredService<MainWindowsModel>();
            
            try
            {
                if (Dispatcher != null) 
                    await Dispatcher.InvokeAsync(new Action(() => DataContext = _model));
                await _model.BeginLoad();
            }
            catch (Exception exception)
            {
                MessageBox.Show(this, exception.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void Unistall_OnClick(object sender, RoutedEventArgs e) 
            => await _model.UnInstall();

        private async void Install_OnClick(object sender, RoutedEventArgs e) 
            => await _model.Install();
    }
}
