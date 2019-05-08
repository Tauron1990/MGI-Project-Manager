using Microsoft.EntityFrameworkCore;
using Tauron.MgiProjectManager.Data.Models;

namespace Tauron.MgiProjectManager.Data.Contexts
{
    public sealed class FilesDbContext : DbContext
    {
        public FilesDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<FileBlobInfoEntity> FileInfos => Set<FileBlobInfoEntity>();

        public DbSet<FileBlobEntity> Blobs => Set<FileBlobEntity>();
    }
}