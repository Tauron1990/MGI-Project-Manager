using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Options;
using Tauron.CQRS.Common.Configuration;

namespace Tauron.CQRS.Server.EventStore
{
    [UsedImplicitly]
    public class DesignTimeContext : IDesignTimeDbContextFactory<DispatcherDatabaseContext>
    {
        public DispatcherDatabaseContext CreateDbContext(string[] args) 
            => new DispatcherDatabaseContext(new OptionsWrapper<ServerConfiguration>(new ServerConfiguration().WithDatabase("Data Source=localhost\\SQLEXPRESS;Initial Catalog=Dispatcher;Integrated Security=True;")));
    }
}