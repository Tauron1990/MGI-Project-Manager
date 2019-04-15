using Microsoft.EntityFrameworkCore;
using Tauron.MgiProjectManager.Data.Models;

namespace Tauron.MgiProjectManager.Data.Contexts
{
    public sealed class FilesDbContext : DbContext
    {
        public DbSet<FileBlobInfoEntity> FileInfos => Set<FileBlobInfoEntity>();

        public FilesDbContext(DbContextOptions options) : base(options)
        {

        }
    }
}