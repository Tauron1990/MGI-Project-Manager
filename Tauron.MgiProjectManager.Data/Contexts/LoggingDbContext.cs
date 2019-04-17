using System;
using Microsoft.EntityFrameworkCore;
using Tauron.MgiProjectManager.Data.Models;

namespace Tauron.MgiProjectManager.Data.Contexts
{
    public class LoggingDbContext : DbContext
    {
        public static Action<DbContextOptionsBuilder> ConnectionBuilder { get; set; }

        public DbSet<LoggingEventEntity> Events => Set<LoggingEventEntity>();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            ConnectionBuilder(optionsBuilder);
            base.OnConfiguring(optionsBuilder);
        }
    }
}