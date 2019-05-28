using CQRSlite.Caching;
using CQRSlite.Domain;
using CQRSlite.Events;
using CQRSlite.Snapshotting;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Tauron.CQRS.Common.Configuration;
using Tauron.CQRS.Services.Core;

namespace Tauron.CQRS.Services.Extensions
{
    [PublicAPI]
    public static class Extensions
    {
        public static void AddCQRSServices(this IServiceCollection services)
        {
            services.AddCQRSTypeHandling();
            services.TryAddSingleton<IDispatcherClient, DispatcherClient>();
            services.TryAddTransient<IDispatcherApi, DispatcherApi>();
            services.TryAddSingleton<IEventStore, ServerEventStore>();
            services.TryAddSingleton<ICache, MemoryCache>();
            services.AddScoped<ISession, Session>();
            services.AddScoped<IRepository>(s =>
                new SnapshotRepository(
                    s.GetRequiredService<ISnapshotStore>(),
                    new DefaultSnapshotStrategy(),
                    new CacheRepository(
                        s.GetRequiredService<IRepository>(),
                        s.GetRequiredService<IEventStore>(),
                        s.GetRequiredService<ICache>()),
                    s.GetRequiredService<IEventStore>()));
        }
    }
}