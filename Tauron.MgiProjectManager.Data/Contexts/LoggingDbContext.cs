using Microsoft.EntityFrameworkCore;
using Tauron.MgiProjectManager.Data.Models;

namespace Tauron.MgiProjectManager.Data.Contexts
{
    public class LoggingDbContext : DbContext
    {
        public static string ConnectionString { get; set; }

        public DbSet<LoggingEventEntity> Events => Set<LoggingEventEntity>();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(ConnectionString);
            base.OnConfiguring(optionsBuilder);
        }
    }
}