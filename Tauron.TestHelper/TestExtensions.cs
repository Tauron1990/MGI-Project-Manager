using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Tauron.CQRS.Common.Configuration;
using Tauron.CQRS.Services;
using Tauron.CQRS.Services.Extensions;
using Tauron.TestHelper.Core;

namespace Tauron.TestHelper
{
    public static class TestExtensions
    {
        public static async Task<IServiceProvider> AddTestCqrs(this IServiceCollection collection, Action<ClientCofiguration> config)
        {
            collection.AddCQRSServices(config);
            collection.Replace(new ServiceDescriptor(typeof(IDispatcherClient), typeof(TestDispatcher), ServiceLifetime.Singleton));

            var provider = collection.BuildServiceProvider();
            await provider.StartCQRS();

            return provider;
        }
    }
}
