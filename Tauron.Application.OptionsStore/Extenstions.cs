using System;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Raven.Client.Documents;
using Tauron.Application.OptionsStore.Data;
using Tauron.Application.OptionsStore.Data.RavenDb;
using Tauron.Application.OptionsStore.Store;

namespace Tauron.Application.OptionsStore
{
    public static class Extenstions
    {
        [PublicAPI]
        public static IServiceCollection AddOptionsStore(this IServiceCollection serviceCollection, Func<IServiceProvider, IDocumentStore> creator)
        {
            serviceCollection.TryAddSingleton<IOptionsStore, OptionsStoreImpl>();
            serviceCollection.TryAddSingleton<IDataClient>(s => new RavenDataClient(creator, s));
            return serviceCollection;
        }
    }
}