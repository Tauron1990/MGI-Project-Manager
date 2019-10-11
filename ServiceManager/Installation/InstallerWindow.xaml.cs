using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Microsoft.Extensions.DependencyInjection;
using ServiceManager.Installation.Core;
using ServiceManager.Services;

namespace ServiceManager.Installation
{
    /// <summary>
    ///     Interaktionslogik für IstallerWindow.xaml
    /// </summary>
    public partial class InstallerWindow
    {
        private readonly Dispatcher _dispatcher;
        private readonly IServiceScopeFactory _serviceScopeFactory;


        public InstallerWindow(MainWindow mainWindow, IServiceScopeFactory serviceScopeFactory, Dispatcher dispatcher)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _dispatcher = dispatcher;
            InitializeComponent();

            Owner = mainWindow;
        }

        public string Path { get; set; }

        public string Error { get; private set; }

        public bool Update { get; set; }

        public RunningService RunningService { get; set; }

        private async void InstallerWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                using var scope = _serviceScopeFactory.CreateScope();

                var installerProcedure = scope.ServiceProvider.GetRequiredService<InstallerProcedure>();

                if(Update)
                    installerProcedure.InitUpdate(scope.ServiceProvider);
                else
                    installerProcedure.InitInstall(scope.ServiceProvider);


                await _dispatcher.InvokeAsync(() => DataContext = installerProcedure);

                
                using var context = Update ? InstallerContext.CreateFrom(RunningService, scope) : new InstallerContext(scope, Path);

                await Task.Delay(2_000);

                var error = await installerProcedure.Install(context);
                Error = error;

                if (string.IsNullOrEmpty(error))
                {
                    await Dispatcher.InvokeAsync(() => DialogResult = true);
                    RunningService = context.CreateRunningService();
                }
                else
                    await Dispatcher.InvokeAsync(() => DialogResult = false);
            }
            catch (Exception exception)
            {
                Error = exception.Message;
                await Dispatcher.InvokeAsync(() => DialogResult = false);
            }
        }
    }
}