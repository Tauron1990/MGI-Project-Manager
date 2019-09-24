using System.Threading.Tasks;
using ServiceManager.Services;

namespace ServiceManager.Installation
{
    public interface IInstallerSystem
    {
        Task<RunningService> Install(string path);

        Task Unistall(RunningService service);
    }
}