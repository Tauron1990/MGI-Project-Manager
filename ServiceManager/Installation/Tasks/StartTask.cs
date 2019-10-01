using System.Threading.Tasks;
using ServiceManager.Installation.Core;

namespace ServiceManager.Installation.Tasks
{
    public class StartTask : InstallerTask
    {
        public override object Content { get; }

        public override string Title => "Start";

        public override Task<string> RunInstall(InstallerContext context)
        {
            throw new System.NotImplementedException();
        }
    }
}