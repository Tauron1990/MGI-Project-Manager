using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Tauron.MgiManager.User.Service.Data.Entitys;

namespace Tauron.MgiManager.User.Service.Data
{
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedNoFixedConstructorSignature)]
    public sealed class UserDatabase : DbContext
    {
        public DbSet<ApplicationUser> Users => Set<ApplicationUser>();

        public DbSet<UserRole> UserRoles => Set<UserRole>();

        public DbSet<Claim> Claims => Set<Claim>();

        public UserDatabase(DbContextOptions options)
            : base(options)
        {

        }

    }
}