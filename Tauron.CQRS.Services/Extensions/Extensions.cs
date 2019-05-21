using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Tauron.CQRS.Services.Core;

namespace Tauron.CQRS.Services.Extensions
{
    [PublicAPI]
    public static class Extensions
    {
        public static void AddCQRSServices(this IServiceCollection service)
        {
            service.TryAddSingleton<IDispatcherClient, DispatcherClient>();
        }
    }
}