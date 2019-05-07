using System;
using System.Threading.Tasks;

namespace Tauron.MgiProjectManager.Dispatcher
{
    public interface IBackgroundTaskDispatcher
    {
        Task SheduleTest(Func<Task> task);
    }
}