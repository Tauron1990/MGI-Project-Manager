﻿using System;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Tauron.CQRS.Common.Configuration;
using Tauron.CQRS.Server.Core;
using Tauron.CQRS.Server.Core.Impl;
using Tauron.CQRS.Server.EventStore;
using Tauron.CQRS.Server.Hubs;

namespace Tauron.CQRS.Server.Extension
{
    [PublicAPI]
    public static class ServerRegisterExtensions
    {
        public static IServiceCollection AddCQRS(this IServiceCollection services, Action<ServerConfiguration> configuration)
        {
            services.AddCQRSTypeHandling();

            services.AddSignalR();

            services.TryAddSingleton<IEventManager, EventManager>();
            services.TryAddSingleton<IApiKeyStore, ApiKeyStore>();
            services.TryAddSingleton<IConnectionManager, ConnectionManager>();

            services.AddHostedService<DispatcherService>();
            services.AddHostedService<HeartBeatService>();

            services.AddDbContext<DispatcherDatabaseContext>();
            services.Configure(configuration);

            return services;
        }

        public static IMvcBuilder AddCQRS(this IMvcBuilder builder) 
            => builder.AddApplicationPart(typeof(ServerRegisterExtensions).Assembly);
    }
}