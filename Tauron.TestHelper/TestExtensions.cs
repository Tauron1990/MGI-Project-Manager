using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Tauron.CQRS.Common.Configuration;
using Tauron.CQRS.Services;
using Tauron.CQRS.Services.Core;
using Tauron.CQRS.Services.Extensions;
using Tauron.TestHelper.Core;
using Tauron.TestHelper.Data;

namespace Tauron.TestHelper
{
    [PublicAPI]
    public static class TestExtensions
    {
        public static async Task<IServiceProvider> AddTestCqrs(this IServiceCollection collection, InMemoryDatabaseRoot root, Action<ClientCofiguration> config)
        {
            collection.AddDbContext<DataStore>(builder => builder.UseInMemoryDatabase(nameof(DataStore), root));
            collection.AddCQRSServices(config);

            collection.Replace(new ServiceDescriptor(typeof(IDispatcherClient), typeof(TestDispatcher), ServiceLifetime.Singleton));
            collection.Replace(new ServiceDescriptor(typeof(IEventServerApi), typeof(TestEventApi), ServiceLifetime.Transient));
            collection.Replace(new ServiceDescriptor(typeof(IPersistApi), typeof(TestPersistApi), ServiceLifetime.Transient));

            var provider = collection.BuildServiceProvider();
            await provider.StartCQRS();

            return provider;
        }
    }
}
