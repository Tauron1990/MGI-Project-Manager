using System;
using System.Collections.Generic;
using System.Linq;

namespace Neleus.DependencyInjection.Extensions
{
    internal class ServiceByNameFactory<TService, TMeta> : IServiceByNameFactoryMeta<TService, TMeta>
    {
        private readonly IDictionary<string, MetaContainer<TMeta>> _registrations;
        private readonly IServiceProvider _serviceProvider;

        public ServiceByNameFactory(IServiceProvider serviceProvider, IDictionary<string, MetaContainer<TMeta>> registrations)
        {
            _serviceProvider = serviceProvider;
            _registrations = registrations;
        }

        public TService GetByName(string name) 
            => GetByName(name, out _);

        public TService GetByName(string name, out TMeta metadata)
        {
            if (!_registrations.TryGetValue(name, out var container))
                throw new ArgumentException($"Service name '{name}' is not registered");
            metadata = container.Meta;
            return (TService)_serviceProvider.GetService(container.Implementation);
        }

        public IEnumerable<TMeta> GetMetadata() 
            => _registrations.Values.Select(v => v.Meta);
    }
}