using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Tauron.MgiProjectManager.Data.Repositorys;
using Tauron.MgiProjectManager.Dispatcher.Actions;

namespace Tauron.MgiProjectManager.BL.Tasks.TimedTasks
{
    [Export(typeof(ITimeTask))]
    public sealed class LoggingMonitor : ITimeTask
    {
        public string Name => nameof(LoggingMonitor);
        public TimeSpan Interval => TimeSpan.FromDays(365);
        public async Task TriggerAsync(IServiceProvider provider) 
            => await provider.GetRequiredService<ILoggingDb>().LimitCount(1_000_000);
    }
}