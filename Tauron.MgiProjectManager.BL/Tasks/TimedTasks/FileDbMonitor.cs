using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Tauron.MgiProjectManager.Data.Repositorys;
using Tauron.MgiProjectManager.Dispatcher;
using Tauron.MgiProjectManager.Dispatcher.Actions;

namespace Tauron.MgiProjectManager.BL.Tasks.TimedTasks
{
    [Export(typeof(ITimeTask))]
    public class FileDbMonitor : ITimeTask
    {
        private readonly IOptions<AppSettings> _appOptions;

        public string Name => nameof(FileDbMonitor);
        public TimeSpan Interval => TimeSpan.FromDays(7);

        public FileDbMonitor(IOptions<AppSettings> appOptions)
        {
            _appOptions = appOptions;
        }

        public async Task TriggerAsync(IServiceProvider provider)
        {
            var maxSize = _appOptions.Value.FilesConfig.MaxDbSize;
            var db = provider.GetRequiredService<IFileDatabase>();
            var dbSize = await db.ComputeSize();

            if(dbSize < maxSize) return;

            var filesToDelete = await db.GetOldestBySize(maxSize - dbSize);

            await db.Delete(filesToDelete);
            await db.SaveChanges();
        }
    }
}