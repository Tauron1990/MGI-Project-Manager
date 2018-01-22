using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NLog.Extensions.Logging;
using Tauron.Application.MgiProjectManager.Data.Entitys;

namespace Tauron.Application.MgiProjectManager.Data
{
    [PublicAPI]
    public sealed class DatabaseImpl : DbContext
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<JobEntity>()
                        .HasOne(e => e.JobRun)
                        .WithOne(b => b.Job)
                        .HasForeignKey<JobRunEntity>(b => b.JobId);
            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring([NotNull] DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLoggerFactory(new NLogLoggerFactory());
            var datasource                                                         = CommonApplication.Current?.GetdefaultFileLocation().CombinePath("jobs.db") ?? "Jobs.db";
            optionsBuilder.UseSqlite(new SqliteConnectionStringBuilder {DataSource = datasource}.ConnectionString);
            base.OnConfiguring(optionsBuilder);
        }
        #pragma warning disable CS3003 // Typ ist nicht CLS-kompatibel
        public DbSet<JobEntity> JobEntities { get; }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public DbSet<JobRunEntity> RunEntities { get; private set; }

        public DbSet<SetupEntity> SetupEntities { get; set; }
        #pragma warning restore CS3003 // Typ ist nicht CLS-kompatibel
    }
}