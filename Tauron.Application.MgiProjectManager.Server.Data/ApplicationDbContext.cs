﻿using JetBrains.Annotations;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Tauron.Application.MgiProjectManager.Server.Data.Entitys;

namespace Tauron.Application.MgiProjectManager.Server.Data
{
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedNoFixedConstructorSignature)]
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<TimedTaskEntity> TimedTasks => Set<TimedTaskEntity>();

        public DbSet<OperationEntity> Operations => Set<OperationEntity>();

        public DbSet<OperationContextEntity> OperationContexts => Set<OperationContextEntity>();

        //public static string ConnectionPath { get; set; } = "C:\\temp.db";

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseSqlServer("Server=(localdb)\\\\mssqllocaldb;Database=aspnet-MGI-Project-Manager-53bc9b9d-9d6a-45d4-8429-2a2761773502;Trusted_Connection=True;MultipleActiveResultSets=true");
        //    base.OnConfiguring(optionsBuilder);
        //}
    }
}