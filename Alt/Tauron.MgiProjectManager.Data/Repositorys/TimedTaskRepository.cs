using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Tauron.MgiProjectManager.Data.Contexts;
using Tauron.MgiProjectManager.Data.Models;

namespace Tauron.MgiProjectManager.Data.Repositorys
{
    public class TimedTaskRepository : ITimedTaskRepository
    {
        private readonly ApplicationDbContext _context;

        public TimedTaskRepository(ApplicationDbContext contextFactory) 
            => _context = contextFactory;

        public async Task<IEnumerable<TimedTaskEntity>> GetTasks() 
            => await _context.TimedTasks.AsNoTracking().ToArrayAsync();

        public async Task<TimedTaskEntity> UpdateTime(string name)
        {
            var ent = await _context.TimedTasks.FindAsync(name);
            ent.LastRun = DateTime.Now;
            return ent;
        }
    }
}