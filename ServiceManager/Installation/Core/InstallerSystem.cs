using System;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ServiceManager.Core;
using ServiceManager.Services;

namespace ServiceManager.Installation.Core
{
    public class InstallerSystem : IInstallerSystem
    {
        private readonly ILogger<InstallerSystem> _logger;
        private readonly ServiceSettings _serviceSettings;
        private readonly IServiceScopeFactory _scopeFactory;

        public InstallerSystem(ILogger<InstallerSystem> logger, ServiceSettings serviceSettings, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _serviceSettings = serviceSettings;
            _scopeFactory = scopeFactory;
        }

        public async Task<RunningService> Install(string path)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var nameWindow = scope.ServiceProvider.GetRequiredService<ValueRequesterWindow>();
                nameWindow.MessageText = "Name des Services?";

                if (nameWindow.Dispatcher == null || await nameWindow.Dispatcher.InvokeAsync(nameWindow.ShowDialog) != true)
                {
                    _logger.LogWarning("No Service Name Entered. Prodcedure canceled");
                }

                using ZipArchive archive = new ZipArchive(File.Open(path, FileMode.Open), ZipArchiveMode.Read, false);

            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error on Install");
            }
        }

        public Task Unistall(RunningService service)
        {
            throw new System.NotImplementedException();
        }
    }
}