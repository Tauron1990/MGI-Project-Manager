using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Tauron.Application;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    [PublicAPI]
    public static class TauronApplicationCommonExtensions
    {
        public static IServiceCollection AddTauronCommon(this IServiceCollection serviceCollection)
        {
            serviceCollection.TryAddScoped<ITauronEnviroment, TauronEnviroment>();
            serviceCollection.TryAddScoped<IEventAggregator, EventAggregator>();
            return serviceCollection;
        }
    }
}