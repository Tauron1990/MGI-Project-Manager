using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Tauron.Application.OptionsStore.Data;
using Tauron.Application.OptionsStore.Data.MongoDb;
using Tauron.Application.OptionsStore.Store;

namespace Tauron.Application.OptionsStore
{
    public static class Extenstions
    {
        [PublicAPI]
        public static IServiceCollection AddOptionsStore(this IServiceCollection serviceCollection)
        {
            serviceCollection.TryAddSingleton<IOptionsStore, OptionsStoreImpl>();
            serviceCollection.TryAddSingleton<IDataClient, MongoDbDataClient>();
            return serviceCollection;
        }
    }
}