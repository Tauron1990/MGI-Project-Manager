using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace Neleus.DependencyInjection.Extensions
{
    /// <summary>
    ///     Provides easy fluent methods for building named registrations of the same interface
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    public class ServicesByNameBuilder<TService, TMeta>
    {
        private readonly IDictionary<string, MetaContainer<TMeta>> _registrations;
        private readonly IServiceCollection _services;

        internal ServicesByNameBuilder(IServiceCollection services, NameBuilderSettings settings)
        {
            _services = services;
            _registrations = settings.CaseInsensitiveNames
                ? new Dictionary<string, MetaContainer<TMeta>>(StringComparer.OrdinalIgnoreCase)
                : new Dictionary<string, MetaContainer<TMeta>>();
        }

        /// <summary>
        ///     Maps name to corresponding implementation.
        ///     Note that this implementation has to be also registered in IoC container so
        ///     that <see cref="IServiceByNameFactory&lt;TService&gt;" /> is be able to resolve it.
        /// </summary>
        public ServicesByNameBuilder<TService, TMeta> Add(string name, Type implemtnationType)
        {
            return Add(name, implemtnationType, default);
        }

        /// <summary>
        ///     Generic version of <see cref="Add" />
        /// </summary>
        public ServicesByNameBuilder<TService, TMeta> Add<TImplementation>(string name)
            where TImplementation : TService
        {
            return Add(name, typeof(TImplementation));
        }

        /// <summary>
        ///     Maps name to corresponding implementation.
        ///     Note that this implementation has to be also registered in IoC container so
        ///     that <see cref="IServiceByNameFactory&lt;TService&gt;" /> is be able to resolve it.
        /// </summary>
        public ServicesByNameBuilder<TService, TMeta> Add(string name, Type implemtnationType, TMeta meta)
        {
            _registrations.Add(name, new MetaContainer<TMeta>(implemtnationType) { Meta = meta });
            return this;
        }

        /// <summary>
        ///     Generic version of <see cref="Add" />
        /// </summary>
        public ServicesByNameBuilder<TService, TMeta> Add<TImplementation>(string name, TMeta meta)
            where TImplementation : TService
        {
            return Add(name, typeof(TImplementation), meta);
        }

        /// <summary>
        ///     Adds <see cref="IServiceByNameFactory&lt;TService&gt;" /> to IoC container together with all registered
        ///     implementations
        ///     so it can be consumed by client code later. Note that each implementation has to be also registered in IoC
        ///     container so
        ///     <see cref="IServiceByNameFactory&lt;TService&gt;" /> is be able to resolve it from the container.
        /// </summary>
        public void Build()
        {
            var registrations = _registrations;
            //Registrations are shared across all instances

            _services.AddTransient<IServiceByNameFactory<TService>>(s => new ServiceByNameFactory<TService, TMeta>(s, registrations));
            _services.AddTransient<IServiceByNameFactoryMeta<TService, TMeta>>(s => new ServiceByNameFactory<TService, TMeta>(s, registrations));
        }
    }
}