using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Tauron.ServiceBootstrapper
{
    public static class BootStrapper
    {
        private static ManualResetEvent _exitWaiter = new ManualResetEvent(false);

        public async static Task Enter<TStartOptions>(string[] args, Func<IServiceProvider, TStartOptions, Task<bool>> startUp = null)
        {
            var collection = new ServiceCollection();

            collection.AddLogging(lb => lb.)

            _exitWaiter.WaitOne();

            _exitWaiter.Dispose();
        }
    }
}
