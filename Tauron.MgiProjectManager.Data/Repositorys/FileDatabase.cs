using Tauron.MgiProjectManager.Data.Contexts;

namespace Tauron.MgiProjectManager.Data.Repositorys
{
    [Export(typeof(IFileDatabase))]
    public class FileDatabase : IFileDatabase
    {
        private readonly FilesDbContext _context;

        public FileDatabase(FilesDbContext context) 
            => _context = context;
    }
}