using System;
using System.Threading.Tasks;

namespace Tauron.MgiProjectManager.Dispatcher
{
    public interface IBackgroundTaskDispatcher
    {
        Task SheduleTask(Func<IServiceProvider, Task> task);
    }
}