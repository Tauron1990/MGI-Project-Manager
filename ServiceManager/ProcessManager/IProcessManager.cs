using System.Threading.Tasks;
using ServiceManager.Services;

namespace ServiceManager.ProcessManager
{
    public interface IProcessManager
    {
        Task Start(RunningService service);

        Task Stop(RunningService service, int timeToKill);
    }
}