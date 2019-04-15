using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Tauron.MgiProjectManager.Data.Contexts;
using Tauron.MgiProjectManager.Data.Models;

namespace Tauron.MgiProjectManager.Data.Repositorys
{
    public class OperationRepository : IOperationRepository
    {
        private readonly ApplicationDbContext _context;

        public OperationRepository(ApplicationDbContext contextBuilder)
            => _context = contextBuilder;

        public async Task<IEnumerable<OperationEntity>> GetAllOperations()
        {
            return await _context.Operations.AsNoTracking().Include(oe => oe.Context).Where(o => !o.Compled).ToArrayAsync();
        }

        public Task<OperationEntity> Find(string id) 
            => _context.Operations.AsNoTracking().Include(e => e.Context).FirstOrDefaultAsync(e => e.OperationId == id);

        public async Task AddOperation(OperationEntity entity) => await _context.Operations.AddAsync(entity);

        public async Task CompledOperation(string id)
        {
            var ent = await _context.Operations.FindAsync(id);
            ent.Compled = true;
        }

        public async Task Remove(string id)
        {
            var ent = await _context.Operations.Include(oe => oe.Context).FirstAsync(e => e.OperationId == id);
            ent.Compled = true;
            ent.Removed = true;
        }

        public async Task UpdateOperation(string id, Action<OperationEntity> update)
        {
            var op = await _context.Operations.FindAsync(id);
            update(op);
        }
    }
}