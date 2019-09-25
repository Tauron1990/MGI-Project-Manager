using System.Threading.Tasks;
using ServiceManager.Installation.Core;

namespace ServiceManager.Installation.Tasks
{
    public sealed class NameSelectionTask : InstallerTask
    {
        public override object Content { get; }

        public override string Title { get; }

        public override Task Prepare(InstallerContext context)
        {

        }

        public override Task<string> RunInstall(InstallerContext context)
        {
            throw new System.NotImplementedException();
        }
    }
}