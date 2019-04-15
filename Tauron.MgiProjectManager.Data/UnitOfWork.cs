// =============================
// Email: info@ebenmonney.com
// www.ebenmonney.com/templates
// =============================

using System;
using System.Threading.Tasks;
using Tauron.MgiProjectManager.Data.Contexts;
using Tauron.MgiProjectManager.Data.Repositorys;

namespace Tauron.MgiProjectManager.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        protected UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            TimedTaskRepository = new TimedTaskRepository(context);
            OperationRepository = new OperationRepository(context);
            FileRepository = new FileRepository(context);
        }

        public IFileRepository FileRepository { get; }
        public ITimedTaskRepository TimedTaskRepository { get; }
        public IOperationRepository OperationRepository { get; }
        public async Task<int> SaveChanges() => await _context.SaveChangesAsync();
    }
}
