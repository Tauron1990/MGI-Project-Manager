using System.Threading.Tasks;
using ServiceManager.Services;

namespace ServiceManager.ProcessManager
{
    public interface IProcessManager
    {
        Task<bool> Start(RunningService service);

        Task<bool> Stop(RunningService service, int timeToKill);
    }
}