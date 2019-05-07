using System;
using System.Threading.Tasks;
using Tauron.MgiProjectManager.Dispatcher;

namespace ServerTest.TestHelper
{
    public class SyncTaskSheduler : IBackgroundTaskDispatcher
    {
        public async Task SheduleTest(Func<Task> task) 
            => await task();
    }
}