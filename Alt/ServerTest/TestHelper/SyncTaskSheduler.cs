using System;
using System.Threading.Tasks;
using Tauron.MgiProjectManager.Dispatcher;

namespace ServerTest.TestHelper
{
    public class SyncTaskSheduler : IBackgroundTaskDispatcher
    {
        private readonly IServiceProvider _serviceProvider;

        public SyncTaskSheduler(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task SheduleTask(Func<IServiceProvider, Task> task) => await task(_serviceProvider);
    }
}