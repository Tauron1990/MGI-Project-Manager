using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Tauron.CQRS.Common.Configuration;

namespace Tauron.TestHelper
{
    public static class TestStart
    {
        public static async Task<IServiceProvider> Create(Action<ClientCofiguration> config, Action<ServiceCollection> serviceCollectionConfig = null)
        {
            var coll = new ServiceCollection();

            serviceCollectionConfig?.Invoke(coll);

            return await coll.AddTestCqrs(new InMemoryDatabaseRoot(), config);
        }
    }
}