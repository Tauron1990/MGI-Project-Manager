using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Tauron.Application.Data.Raven.Impl;

namespace Tauron.Application.Data.Raven
{
    [PublicAPI]
    public static class Extensions
    {
        public static DataRavenConfiguration AddDataRaven(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection.Configure<DatabaseOption>(configuration.GetSection("DatabaseOptions"))
               .TryAddSingleton<IDatabaseCache, DatabaseCacheImpl>();

            var config = new DataRavenConfiguration();
            serviceCollection.Configure<MemoryConfig>(c => c.MemoryStores = config.MemoryStores);

            return config;
        }

        public static DataRavenConfiguration AddDataRaven(this IServiceCollection serviceCollection)
        {
            serviceCollection.TryAddSingleton<IDatabaseCache, DatabaseCacheImpl>();

            var config = new DataRavenConfiguration();
            serviceCollection.Configure<MemoryConfig>(c => c.MemoryStores = config.MemoryStores);

            return config;
        }
    }
}