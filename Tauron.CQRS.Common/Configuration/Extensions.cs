using System;
using System.Reflection;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Tauron.CQRS.Common.Dto;
using Tauron.CQRS.Common.Dto.TypeHandling;

namespace Tauron.CQRS.Common.Configuration
{
    [PublicAPI]
    public static class Extensions
    {
        public static void AddCQRSTypeHandling(this IServiceCollection serviceCollection)
        {
            TypeResolver.TypeRegistry.Register(nameof(EventFailedEventMessage), typeof(EventFailedEventMessage));
            serviceCollection.AddSingleton(provider => TypeResolver.TypeRegistry);
        }

        public static void AddCQRSTypeHandling(this IServiceCollection serviceCollection, Action<CommonConfiguration> config)
        {
            serviceCollection.AddCQRSTypeHandling();
            var configInst = new CommonConfiguration();
            config(configInst);
        }

        public static void ScanTypesFrom<TType>(this CommonConfiguration configuration)
        {
            foreach (var exportedType in typeof(TType).Assembly.GetExportedTypes())
            {
                if (!(exportedType.GetCustomAttribute(typeof(DtoAttribute)) is DtoAttribute attribute)) continue;

                configuration.RegisterType(attribute.Name, attribute.Type ?? exportedType);
            }
        }
    }
}