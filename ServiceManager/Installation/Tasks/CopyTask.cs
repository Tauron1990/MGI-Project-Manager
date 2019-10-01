using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ServiceManager.Core;
using ServiceManager.Installation.Core;

namespace ServiceManager.Installation.Tasks
{
    public sealed class CopyTask : InstallerTask
    {
        private readonly ILogger<CopyTask> _logger;
        private object _content;

        public override object Content => _content;

        public override string Title => "Daten Kopieren";

        public CopyTask(ILogger<CopyTask> logger)
        {
            _logger = logger;
        }

        public override async Task Prepare(InstallerContext context)
        {
            var dispatcher = context.ServiceScope.ServiceProvider.GetRequiredService<Dispatcher>();
            _content = await dispatcher.InvokeAsync(() => new TextBlock
                                                          {
                                                              TextAlignment = TextAlignment.Center,
                                                              TextWrapping = TextWrapping.Wrap,
                                                              Text = $"Daten werden für {context.ServiceName} Kopiert"
                                                          });
        }

        public override Task<string> RunInstall(InstallerContext context)
        {
            var path = Path.Combine("Apps", context.ServiceName).ToApplicationPath();
            if (!Directory.Exists(path))
            {
                _logger.LogInformation($"{context.ServiceName}: Create Service Directory");
                Directory.CreateDirectory(path);
            }

            context.PackageArchive.ExtractToDirectory(path, true);

            _logger.LogInformation($"{context.ServiceName}: Extraction Compled");
            return Task.FromResult<string>(null);
        }
    }
}