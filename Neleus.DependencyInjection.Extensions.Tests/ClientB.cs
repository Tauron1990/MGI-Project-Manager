using System.Collections.Generic;

namespace Neleus.DependencyInjection.Extensions.Tests
{
    /// <summary>
    ///     This client is not aware of name and has no dependency on <see cref="IServiceByNameFactory" />. The IEnumerable
    ///     <int> dependency injected by IoC factory registration.
    /// </summary>
    public class ClientB
    {
        public ClientB(IEnumerable<int> dependency)
        {
            Dependency = dependency;
        }

        public IEnumerable<int> Dependency { get; }
    }
}