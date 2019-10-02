using System.Threading.Tasks;
using ServiceManager.Installation.Core;
using ServiceManager.ProcessManager;

namespace ServiceManager.Installation.Tasks
{
    public class StartTask : InstallerTask
    {
        private readonly IProcessManager _processManager;
        private object _content;

        public override object Content => _content;

        public override string Title => "Start";

        public StartTask(IProcessManager processManager) 
            => _processManager = processManager;

        public override Task Prepare(InstallerContext context)
        {
            _content = $"Starte Service: {context.ServiceName}";

            return base.Prepare(context);
        }

        public override async Task<string> RunInstall(InstallerContext context)
        {
            if(await _processManager.Start(context.CreateRunningService())) return null;

            return "Programm Konnte nicht gestartet Werden";
        }
    }
}