using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Tauron.MgiProjectManager.Data.Repositorys;
using Tauron.MgiProjectManager.Dispatcher;
using Tauron.MgiProjectManager.Dispatcher.Actions;

namespace Tauron.MgiProjectManager.BL.Tasks.TimedTasks
{
    [Export(typeof(ITimeTask))]
    public class FileDbMonitor : ITimeTask
    {
        private const long MaxSize = 96_636_764_160;

        public string Name => nameof(FileDbMonitor);
        public TimeSpan Interval => TimeSpan.FromDays(7);
        public async Task TriggerAsync(IServiceProvider provider)
        {
            var db = provider.GetRequiredService<IFileDatabase>();
            var dbSize = await db.ComputeSize();

            if(dbSize < MaxSize) return;

            var filesToDelete = await db.GetOldestBySize(MaxSize - dbSize);

            await db.Delete(filesToDelete);
            await db.SaveChanges();
        }
    }
}