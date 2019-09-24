using System.Threading.Tasks;
using ServiceManager.Services;

namespace ServiceManager.Installation.Core
{
    public class InstallerSystem : IInstallerSystem
    {
        public Task<RunningService> Install(string path)
        {
            throw new System.NotImplementedException();
        }

        public Task Unistall(RunningService service)
        {
            throw new System.NotImplementedException();
        }
    }
}