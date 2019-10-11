using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ServiceManager.Installation.Core;
using ServiceManager.ProcessManager;
using ServiceManager.Services;

namespace ServiceManager.Installation.Tasks
{
    public sealed class StopTask : InstallerTask
    {
        private readonly IProcessManager _processManager;
        private readonly ILogger<StopTask> _logger;

        public override string Title => "Service Stop";

        public StopTask(IProcessManager processManager, ILogger<StopTask> logger)
        {
            _processManager = processManager;
            _logger = logger;
        }

        public override Task Prepare(InstallerContext context)
        {
            Content = "Service wird Gestoppt...";

            return Task.CompletedTask;
        }

        public override async Task<string> RunInstall(InstallerContext context)
        {
            var service = context.CreateRunningService();

            if (service.ServiceStade == ServiceStade.Running)
            {
                if (await _processManager.Stop(service, 10_000))
                {
                    _logger.LogWarning($"{service.Name}: Stopping failed");
                    return "Das Stoppen des Services ist Fehlgeschlagen";
                }
            }
        }
    }
}