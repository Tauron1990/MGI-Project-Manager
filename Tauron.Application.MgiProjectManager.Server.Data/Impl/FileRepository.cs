using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Tauron.Application.MgiProjectManager.Server.Data.Entitys;
using Tauron.Application.MgiProjectManager.Server.Data.Repository;

namespace Tauron.Application.MgiProjectManager.Server.Data.Impl
{
    public class FileRepository : IFileRepository
    {
        private readonly Func<ApplicationDbContext> _contextFactory;

        public FileRepository(Func<ApplicationDbContext> contextFactory) 
            => _contextFactory = contextFactory;

        public async Task AddFile(FileEntity entity)
        {
            using (var context = _contextFactory())
            {
                await context.AddAsync(entity);
                await context.SaveChangesAsync();
            }
        }

        public async Task<FileEntity[]> GetUnRequetedFiles()
        {
            using (var context = _contextFactory())
                return await context.Files.AsNoTracking().Where(fe => !fe.IsDeleted && !fe.IsRequested).ToArrayAsync();
        }

        public async Task DeleteFile(int id)
        {
            using (var context = _contextFactory())
            {
                var ent = await context.Files.FindAsync(id);
                ent.IsDeleted = true;
                await context.SaveChangesAsync();
            }
        }
    }
}