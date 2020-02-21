using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;

namespace Tauron.Application.Shared
{
    [PublicAPI]
    public static class SheardExtensions
    {
        public static IServiceCollection AddModules(this IServiceCollection serviceCollection, params DIModule[] modules)
        {
            foreach (var module in modules)
            {
                module.ServiceCollection = serviceCollection;
                module.Load();
            }

            return serviceCollection;
        }
    }
}