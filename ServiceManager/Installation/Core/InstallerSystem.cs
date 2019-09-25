using System.Threading.Tasks;
using System.Windows.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ServiceManager.Services;

namespace ServiceManager.Installation.Core
{
    public class InstallerSystem : IInstallerSystem
    {
        private readonly ILogger<InstallerSystem> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly Dispatcher _dispatcher;

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

            //try
            //{
            //    using var scope = _scopeFactory.CreateScope();
            //    var nameWindow = scope.ServiceProvider.GetRequiredService<ValueRequesterWindow>();
            //    nameWindow.MessageText = "Name des Services?";

            //    if (nameWindow.Dispatcher == null || await nameWindow.Dispatcher.InvokeAsync(nameWindow.ShowDialog) != true)
            //    {
            //        _logger.LogWarning("No Service Name Entered. Prodcedure canceled");
            //        return null;
            //    }

            //    using ZipArchive archive = new ZipArchive(File.Open(path, FileMode.Open), ZipArchiveMode.Read, false);

            //    return null;
            //}
            //catch (Exception e)
            //{
            //    _logger.LogError(e, "Error on Install");
            //}

            //return null;
        }

        public Task Unistall(RunningService service)
        {
            throw new System.NotImplementedException();
        }
    }
}