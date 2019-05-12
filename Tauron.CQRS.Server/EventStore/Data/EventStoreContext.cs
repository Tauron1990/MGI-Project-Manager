using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Tauron.CQRS.Common.Configuration;

namespace Tauron.CQRS.Server.EventStore.Data
{
    public class EventStoreContext : DbContext
    {
        private readonly IOptions<ServerConfiguration> _serverOptions;

        public DbSet<EventEntity> EventEntities { get; set; }

        public EventStoreContext(IOptions<ServerConfiguration> serverOptions) 
            => _serverOptions = serverOptions;

        #region Overrides of DbContext

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_serverOptions.Value.ConnectionString, builder => builder.EnableRetryOnFailure());
            base.OnConfiguring(optionsBuilder);
        }

        #endregion
    }
}