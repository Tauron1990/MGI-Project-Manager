using Microsoft.EntityFrameworkCore;
using Tauron.CQRS.Common.Dto.Persistable;
using Tauron.CQRS.Common.ServerHubs;

namespace Tauron.TestHelper.Data
{
    public sealed class DataStore : DbContext
    {
        public DataStore(DbContextOptions options)
            : base(options)
        {
            
        }

        public DbSet<ServerDomainMessage> Messages => Set<ServerDomainMessage>();

        public DbSet<ObjectStade> ObjectStades => Set<ObjectStade>();
    }
}