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
        public static IServiceCollection AddDataRaven(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection.Configure<DatabaseOption>(configuration.GetSection("DatabaseOptions"))
               .TryAddSingleton<IDatabaseCache, DatabaseCacheImpl>();

            return serviceCollection;
        }
    }
}