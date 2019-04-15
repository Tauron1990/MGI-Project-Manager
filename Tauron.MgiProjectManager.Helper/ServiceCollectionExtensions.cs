using System;
using System.Linq;
using System.Reflection;
using AutoMapper;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;

namespace Tauron.MgiProjectManager
{
    [PublicAPI]
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection ConfigurateMapper(this IServiceCollection collection, params Type[] typeFromAssemblysWithProfiles)
        {
            Mapper.Initialize(config =>
            {
                config.AddProfiles(typeFromAssemblysWithProfiles);
            });

            collection.AddSingleton(Mapper.Instance);
            return collection;
        }

        public static IServiceCollection ImportFrom<TType>(this IServiceCollection serviceCollection)
        {
            foreach (var exportedType in typeof(TType).Assembly.GetExportedTypes().Select(t => t.GetTypeInfo()))
            {
                if(!(exportedType.GetCustomAttribute(typeof(ExportAttribute)) is ExportAttribute attr))
                    continue;

                Func<IServiceProvider, object> factory = null;
                if (!string.IsNullOrWhiteSpace(attr.Factory))
                {
                    var methodInfo = exportedType.GetMethod(attr.Factory);
                    var param = methodInfo?.GetParameters();

                    if (methodInfo != null && methodInfo.ReturnType != typeof(void) && param.Length == 1 && param[0].ParameterType == typeof(IServiceProvider))
                        factory = (Func<IServiceProvider, object>) Delegate.CreateDelegate(typeof(Func<IServiceProvider, object>), methodInfo);

                }

                if (attr.CreateInstance)
                    serviceCollection.Add(new ServiceDescriptor(attr.ExportType, factory?.Invoke(null) ?? Activator.CreateInstance(exportedType)));

                ServiceDescriptor CreateDescriptor(ServiceLifetime serviceLifetime)
                {
                    return factory != null ? 
                        new ServiceDescriptor(attr.ExportType, factory, serviceLifetime) : 
                        new ServiceDescriptor(attr.ExportType, exportedType, serviceLifetime);
                }

                switch (attr.LiveCycle)
                {
                    case LiveCycle.Scoped:
                        serviceCollection.Add(CreateDescriptor(ServiceLifetime.Scoped));
                        break;
                    case LiveCycle.Transistent:
                        serviceCollection.Add(CreateDescriptor(ServiceLifetime.Transient));
                        break;
                    case LiveCycle.Singleton:
                        serviceCollection.Add(CreateDescriptor(ServiceLifetime.Singleton));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(attr.LiveCycle));
                }
            }

            return serviceCollection;
        }
    }
}