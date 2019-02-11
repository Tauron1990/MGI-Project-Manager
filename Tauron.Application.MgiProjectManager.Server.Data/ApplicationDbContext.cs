using JetBrains.Annotations;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Tauron.Application.MgiProjectManager.Server.Data
{
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedNoFixedConstructorSignature)]
    public class ApplicationDbContext : IdentityDbContext
    {
        public static string ConnectionPath { get; set; } = "C:\\temp.db";
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(new SqliteConnectionStringBuilder {DataSource = ConnectionPath}.ConnectionString);
            base.OnConfiguring(optionsBuilder);
        }
    }
}
