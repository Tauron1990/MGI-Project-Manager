using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Tauron.CQRS.Common.Configuration;

namespace Tauron.CQRS.Services.Data.Database
{
    public class EventSourceContext : DbContext
    {
        private readonly IOptions<ClientCofiguration> _serverOptions;

        public DbSet<EventEntity> EventEntities { get; set; }

        public DbSet<ObjectStadeEntity> ObjectStades { get; set; }

        public EventSourceContext(IOptions<ClientCofiguration> serverOptions) 
            => _serverOptions = serverOptions;

        #region Overrides of DbContext

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (_serverOptions.Value.Memory)
                optionsBuilder.UseInMemoryDatabase(_serverOptions.Value.ConnectionString);
            else
                optionsBuilder.UseSqlServer(_serverOptions.Value.ConnectionString, builder => builder.EnableRetryOnFailure());
            base.OnConfiguring(optionsBuilder);
        }

        #endregion
    }
}