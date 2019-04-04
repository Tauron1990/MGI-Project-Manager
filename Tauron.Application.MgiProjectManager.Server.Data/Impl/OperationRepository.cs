using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Tauron.Application.MgiProjectManager.Server.Data.Entitys;
using Tauron.Application.MgiProjectManager.Server.Data.Repository;

namespace Tauron.Application.MgiProjectManager.Server.Data.Impl
{
    public class OperationRepository : IOperationRepository
    {
        private readonly Func<ApplicationDbContext> _contextBuilder;

        public OperationRepository(Func<ApplicationDbContext> contextBuilder) 
            => _contextBuilder = contextBuilder;

        public async Task<IEnumerable<OperationEntity>> GetAllOperations()
        {
            using (var context = _contextBuilder())
                return await context.Operations.AsNoTracking().Include(oe => oe.Context).Where(o => !o.Compled).ToArrayAsync();
        }

        public async Task AddOperation(OperationEntity entity)
        {
            using (var context = _contextBuilder())
            {
                await context.Operations.AddAsync(entity);
                await context.SaveChangesAsync();
            }
        }

        public async Task CompledOperation(string id)
        {
            using (var context = _contextBuilder())
            {
                var ent = await context.Operations.FindAsync(id);
                ent.Compled = true;
                await context.SaveChangesAsync();
            }
        }

        public async Task Remove(string id)
        {
            using (var context = _contextBuilder())
            {
                var ent = await context.Operations.FindAsync(id);
                ent.Compled = true;
                ent.Removed = true;
                await context.SaveChangesAsync();
            }
        }
    }
}