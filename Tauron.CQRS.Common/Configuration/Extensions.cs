using Microsoft.Extensions.DependencyInjection;
using Tauron.CQRS.Common.Dto.TypeHandling;

namespace Tauron.CQRS.Common.Configuration
{
    public static class Extensions
    {
        public static void AddCQRSTypeHandling(this IServiceCollection serviceCollection) 
            => serviceCollection.AddSingleton<ITypeRegistry>(provider => TypeResolver.TypeRegistry);
    }
}