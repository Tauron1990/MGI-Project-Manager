using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Tauron.MgiProjectManager.Dispatcher
{
    [Export(typeof(IBackgroundTaskDispatcher))]
    public class BackgroundTaskDispatcher : IBackgroundTaskDispatcher
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<IBackgroundTaskDispatcher> _logger;

        public BackgroundTaskDispatcher(IServiceProvider serviceProvider, ILogger<IBackgroundTaskDispatcher> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public Task SheduleTask(Func<IServiceProvider, Task> task)
        {
            Task.Run(async () =>
            {
                using var scope = _serviceProvider.CreateScope();

                await task(scope.ServiceProvider);
            }).ContinueWith(t =>
            {
                if (t.IsFaulted)
                    _logger.LogError(t.Exception?.InnerExceptions.FirstOrDefault(), "Baground Task Failed");
            });

            return Task.CompletedTask;
        }
    }
}