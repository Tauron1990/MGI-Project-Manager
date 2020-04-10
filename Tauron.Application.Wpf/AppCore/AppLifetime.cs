using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Tauron.Application.Host;

namespace Tauron.Application.Wpf.AppCore
{
    public sealed class AppLifetime : IAppRoute
    {
        private readonly IServiceScopeFactory _factory;
        private readonly IHostEnvironment _hostEnvironment;
        private readonly IHostApplicationLifetime _applicationLifetime;
        private System.Windows.Application? _internalApplication;
        private readonly TaskCompletionSource<int> _shutdownWaiter = new TaskCompletionSource<int>();

        public AppLifetime(IServiceScopeFactory factory, IHostEnvironment hostEnvironment, IHostApplicationLifetime applicationLifetime)
        {
            _factory = factory;
            _hostEnvironment = hostEnvironment;
            _applicationLifetime = applicationLifetime;
        }

        public Task WaitForStartAsync(CancellationToken cancellationToken)
        {
            Task.Run(() =>
            {
                using var scope = _factory.CreateScope();
                IOCReplacer.SetServiceProvider(scope.ServiceProvider);

                _internalApplication = scope.ServiceProvider.GetService<IAppFactory>()?.Create() ?? new System.Windows.Application();

                _internalApplication.Startup += (sender, args) =>
                {
                    // ReSharper disable AccessToDisposedClosure
                    var splash = scope.ServiceProvider.GetService<ISplashScreen>()?.Window;
                    splash?.Show();

                    scope.ServiceProvider.GetService<WpfStartup>()?.Configure(_internalApplication, _hostEnvironment);

                    var mainWindow = scope.ServiceProvider.GetRequiredService<IMainWindow>();
                    mainWindow.Window.Show();
                    mainWindow.Shutdown += (o, eventArgs) => _applicationLifetime.StopApplication();

                    splash?.Hide();
                    // ReSharper restore AccessToDisposedClosure
                };

                _shutdownWaiter.SetResult(_internalApplication.Run());
            }, cancellationToken);

            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _internalApplication?.Shutdown();
            var result = await _shutdownWaiter.Task;
            Environment.ExitCode = result;
        }
    }
}