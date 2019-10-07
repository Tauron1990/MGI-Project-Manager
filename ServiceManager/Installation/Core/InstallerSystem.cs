using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ServiceManager.ProcessManager;
using ServiceManager.Services;

namespace ServiceManager.Installation.Core
{
    public class InstallerSystem : IInstallerSystem
    {
        private readonly Dispatcher _dispatcher;
        private readonly IProcessManager _processManager;
        private readonly MainWindow _mainWindow;
        private readonly ILogger<InstallerSystem> _logger;
        private readonly IServiceScopeFactory _scopeFactory;

        public InstallerSystem(ILogger<InstallerSystem> logger, IServiceScopeFactory scopeFactory, Dispatcher dispatcher, IProcessManager processManager, MainWindow mainWindow)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
            _dispatcher = dispatcher;
            _processManager = processManager;
            _mainWindow = mainWindow;
        }

        public async Task<RunningService> Install(string path)
        {
            using var scope = _scopeFactory.CreateScope();

            var window = scope.ServiceProvider.GetRequiredService<InstallerWindow>();

            window.Path = path;

            if (await _dispatcher.InvokeAsync(() => window.ShowDialog()) != true) return null;

            _logger.LogInformation("Install Completed");
            return window.RunningService;
        }

        public Task<bool?> Unistall(RunningService service)
        {
            return _dispatcher.InvokeAsync(() =>
            {
                using var scope = _scopeFactory.CreateScope();
                var window = scope.ServiceProvider.GetRequiredService<UnistallWindow>();
                window.Owner = _mainWindow;

                window.StartEvent += async () =>
                {
                    switch (service.ServiceStade)
                    {
                        case ServiceStade.Running:
                            if (await _processManager.Stop(service, 20_000))
                                return DeleteService(service, window);
                            else
                            {
                                MessageBox.Show(window, "Service Konnte nicht gestopt werden", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                                return false;
                            }
                        case ServiceStade.Error:
                        case ServiceStade.Ready:
                            return DeleteService(service, window);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                };

                return window.ShowDialog();
            }).Task;
        }

        private static bool DeleteService(RunningService service, UnistallWindow window)
        {
            try
            {
                Directory.Delete(service.InstallationPath, true);
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show(window, $"Fehler beim Löschen: \n {e.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }
    }
}