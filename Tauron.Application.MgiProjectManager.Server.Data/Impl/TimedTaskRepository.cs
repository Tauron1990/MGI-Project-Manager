using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Tauron.Application.MgiProjectManager.Server.Data.Entitys;
using Tauron.Application.MgiProjectManager.Server.Data.Repository;

namespace Tauron.Application.MgiProjectManager.Server.Data.Impl
{
    public class TimedTaskRepository : ITimedTaskRepository
    {
        private readonly Func<ApplicationDbContext> _contextFactory;

        public TimedTaskRepository(Func<ApplicationDbContext> contextFactory) 
            => _contextFactory = contextFactory;

        public async Task<IEnumerable<TimedTaskEntity>> GetTaskAsync()
        {
            using (var context = _contextFactory())
                return await context.TimedTasks.AsNoTracking().ToArrayAsync();
        }

        public async Task<TimedTaskEntity> UpdateTime(string name)
        {
            using (var context = _contextFactory())
            {
                var ent = await context.TimedTasks.FindAsync(name);
                ent.LastRun = DateTime.Now;
                await context.SaveChangesAsync();

                return ent;
            }
        }
    }
}