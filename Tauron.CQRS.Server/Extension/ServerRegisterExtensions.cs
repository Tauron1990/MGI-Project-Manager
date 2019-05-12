using System;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Tauron.CQRS.Common.Configuration;
using Tauron.CQRS.Server.EventStore.Data;

namespace Tauron.CQRS.Server.Extension
{
    [PublicAPI]
    public static class ServerRegisterExtensions
    {
        public static IServiceCollection AddCQRS(this IServiceCollection services, Action<ServerConfiguration> configuration)
        {
            services.AddSignalR();
            services.AddHostedService<DispatcherService>();
            services.AddDbContext<EventStoreContext>();
            services.Configure(configuration);

            return services;
        }

    }
}