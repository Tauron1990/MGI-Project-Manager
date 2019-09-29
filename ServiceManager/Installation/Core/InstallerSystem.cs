using System;
using System.Threading.Tasks;
using System.Windows.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ServiceManager.Services;

namespace ServiceManager.Installation.Core
{
    public class InstallerSystem : IInstallerSystem
    {
        private readonly Dispatcher _dispatcher;
        private readonly ILogger<InstallerSystem> _logger;
        private readonly IServiceScopeFactory _scopeFactory;

        public InstallerSystem(ILogger<InstallerSystem> logger, IServiceScopeFactory scopeFactory, Dispatcher dispatcher)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
            _dispatcher = dispatcher;
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

        public Task Unistall(RunningService service) => throw new NotImplementedException();
    }
}