using System;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;
using Tauron.Application.ProjectManager.ApplicationServer.Core;
using Tauron.Application.ProjectManager.ApplicationServer.Data.PrivateEntitys;
using Tauron.Application.ProjectManager.Services.Data.Entitys;

namespace Tauron.Application.ProjectManager.ApplicationServer.Data
{
    [PublicAPI]
    public sealed class DatabaseImpl : DbContext
    {
        private static WeakDelegate _testMode;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserEntity>().HasChangeTrackingStrategy(ChangeTrackingStrategy.ChangingAndChangedNotifications);
            modelBuilder.Entity<JobRunEntity>().HasChangeTrackingStrategy(ChangeTrackingStrategy.ChangingAndChangedNotifications);
            modelBuilder.Entity<SetupEntity>().HasChangeTrackingStrategy(ChangeTrackingStrategy.ChangingAndChangedNotifications);
            //modelBuilder.Entity<JobEntity>().HasChangeTrackingStrategy(ChangeTrackingStrategy.ChangingAndChangedNotifications);

            //modelBuilder.Entity<JobEntity>()
            //            .HasChangeTrackingStrategy(ChangeTrackingStrategy.ChangingAndChangedNotifications)
            //            .HasOne(e => e.JobRuns)
            //            .WithOne(b => b.)
            //            .HasForeignKey<JobRunEntity>(b => b.JobId);
            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring([NotNull] DbContextOptionsBuilder optionsBuilder)
        {
            if (_testMode == null)
            {
                //optionsBuilder.UseLoggerFactory(new NLogLoggerFactory());
                var datasource = CommonApplication.Current?.GetdefaultFileLocation().CombinePath("jobs.db") ?? "Jobs.db";
                datasource.CreateDirectoryIfNotExis();

                optionsBuilder.UseSqlite(new SqliteConnectionStringBuilder {DataSource = datasource}.ConnectionString);
            }
            else
                _testMode.Invoke(optionsBuilder);

            base.OnConfiguring(optionsBuilder);
        }

        public static void UpdateSchema(Action<DbContextOptionsBuilder> config = null)
        {
            if (config != null)
                _testMode = new WeakDelegate(config);
            else
            {
                _testMode = null;

                using (var db = new DatabaseImpl())
                {
                    Batteries_V2.Init();
                    db.Database.Migrate();
                    UserManager.AddInitial(db);
                    db.SaveChanges();
                }
            }
        }
        #pragma warning disable CS3003 // Typ ist nicht CLS-kompatibel
        public DbSet<JobEntity> Jobs { get; }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public DbSet<JobRunEntity> Runs { get; private set; }

        public DbSet<SetupEntity> Setups { get; set; }

        public DbSet<UserEntity> Users { get; set; }

        public DbSet<OptionEntity> Options { get; set; }
        #pragma warning restore CS3003 // Typ ist nicht CLS-kompatibel
    }
}