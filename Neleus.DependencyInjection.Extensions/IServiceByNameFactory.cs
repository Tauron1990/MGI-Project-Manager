using System.Collections.Generic;
using System.Reflection;

namespace Neleus.DependencyInjection.Extensions
{
    /// <summary>
    ///     Provides instances of registered services by name
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    public interface IServiceByNameFactoryMeta<out TService, TMeta> : IServiceByNameFactory<TService>
    {
        /// <summary>
        ///     Provides instance of registered service by name
        /// </summary>
        TService GetByName(string name, out TMeta metadata);

        IEnumerable<TMeta> GetMetadata();
    }
    public interface IServiceByNameFactory<out TService>
    {
        /// <summary>
        ///     Provides instance of registered service by name
        /// </summary>
        TService GetByName(string name);
    }
}