using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Tauron.CQRS.Health.Core;
using Tauron.CQRS.Health.Core.Impl;

namespace Tauron.CQRS.Health
{
    public static class HealthExtensions
    {
        public static IMvcBuilder AddHealthParts(this IMvcBuilder builder) => builder.AddApplicationPart(typeof(HealthExtensions).Assembly);

        public static void AddHealth(this IServiceCollection serviceCollection)
        {
            serviceCollection.TryAddSingleton<IStatisticsTracker, Tracker>();
            serviceCollection.TryAddScoped<StatisticsMiddleware>();
        }

        public static void UseHealth(this IApplicationBuilder builder) 
            => builder.UseMiddleware<StatisticsMiddleware>();
    }
}