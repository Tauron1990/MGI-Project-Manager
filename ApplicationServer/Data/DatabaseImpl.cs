using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NLog.Extensions.Logging;
using Tauron.Application.ProjectManager.ApplicationServer.Core;
using Tauron.Application.ProjectManager.ApplicationServer.Data.PrivateEntitys;
using Tauron.Application.ProjectManager.Data.Entitys;

namespace Tauron.Application.ProjectManager.ApplicationServer.Data
{
    [PublicAPI]
    public sealed class DatabaseImpl : DbContext
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserEntity>().HasChangeTrackingStrategy(ChangeTrackingStrategy.ChangingAndChangedNotifications);
            modelBuilder.Entity<JobRunEntity>().HasChangeTrackingStrategy(ChangeTrackingStrategy.ChangingAndChangedNotifications);
            modelBuilder.Entity<SetupEntity>().HasChangeTrackingStrategy(ChangeTrackingStrategy.ChangingAndChangedNotifications);

            modelBuilder.Entity<JobEntity>()
                        .HasChangeTrackingStrategy(ChangeTrackingStrategy.ChangingAndChangedNotifications)
                        .HasOne(e => e.JobRun)
                        .WithOne(b => b.Job)
                        .HasForeignKey<JobRunEntity>(b => b.JobId);
            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring([NotNull] DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLoggerFactory(new NLogLoggerFactory());
            var datasource                                                         = CommonApplication.Current?.GetdefaultFileLocation().CombinePath("jobs.db") ?? "Jobs.db";
            datasource.CreateDirectoryIfNotExis();

            optionsBuilder.UseSqlite(new SqliteConnectionStringBuilder {DataSource = datasource}.ConnectionString);
            
            base.OnConfiguring(optionsBuilder);
        }
        #pragma warning disable CS3003 // Typ ist nicht CLS-kompatibel
        public DbSet<JobEntity> Jobs{ get; }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public DbSet<JobRunEntity> Runs { get; private set; }

        public DbSet<SetupEntity> Setups{ get; set; }

        public DbSet<UserEntity> Users { get; set; }
        #pragma warning restore CS3003 // Typ ist nicht CLS-kompatibel

        public static void UpdateSchema()
        {
            using (var db = new DatabaseImpl())
            {
                db.Database.Migrate();
                UserManager.AddInitial(db);
                db.SaveChanges();
            }
        }
    }
}