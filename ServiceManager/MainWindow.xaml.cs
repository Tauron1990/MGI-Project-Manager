using System;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using Microsoft.Extensions.DependencyInjection;
using ServiceManager.ProcessManager;

namespace ServiceManager
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private MainWindowsModel _model;
        private IServiceProvider _serviceProvider;

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

        private void MainWindow_OnClosed(object sender, EventArgs e) => (_serviceProvider as IDisposable)?.Dispose();

        private async void MainWindow_OnClosing(object sender, CancelEventArgs e)
        {
            _model.IsReady = false;
            e.Cancel = true;

            try
            {
                await _serviceProvider.GetRequiredService<IProcessManager>().StopAll();
                Dispatcher?.Invoke(Application.Current.Shutdown);
            }
            catch (TaskCanceledException)
            {
            }
        }
    }
}