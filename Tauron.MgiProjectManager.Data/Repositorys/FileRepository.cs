using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Tauron.MgiProjectManager.Data.Contexts;
using Tauron.MgiProjectManager.Data.Models;

namespace Tauron.MgiProjectManager.Data.Repositorys
{
    public class FileRepository : IFileRepository
    {
        private readonly ApplicationDbContext _context;

        public FileRepository(ApplicationDbContext contextFactory)
            => _context = contextFactory;

        public async Task AddFile(FileEntity entity) 
            => await _context.AddAsync(entity);

        public async Task<FileEntity[]> GetUnRequestedFiles() 
            => await _context.Files.AsNoTracking().Where(fe => !fe.IsDeleted && !fe.IsRequested).ToArrayAsync();

        public async Task DeleteFile(int id)
        {
            var ent = await _context.Files.FindAsync(id);
            ent.IsDeleted = true;
        }
    }
}