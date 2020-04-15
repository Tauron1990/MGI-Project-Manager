using Microsoft.Extensions.DependencyInjection.Extensions;
using Tauron.Application.SoftwareRepo;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    public static class SoftwareExtensions
    {
        public static IServiceCollection AddSoftwareRepo(this IServiceCollection serviceCollection)
        {
            serviceCollection.TryAddTransient<IRepoFactory, RepoFactory>();
            return serviceCollection;
        }
    }
}