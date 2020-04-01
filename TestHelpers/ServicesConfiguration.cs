using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using TestHelpers.Core;

namespace TestHelpers
{
    [PublicAPI]
    public sealed class ServicesConfiguration
    {
        public ServicesConfiguration(IServiceCollection serviceCollection) => ServiceCollection = serviceCollection;

        public IServiceCollection ServiceCollection { get; }

        internal List<ServiceEntry> ServiceEntries { get; } = new List<ServiceEntry>();

        public MockConfiguration<TInterface> CreateMock<TInterface>()
            where TInterface : class
            => new MockConfiguration<TInterface>(this);

        public ServicesConfiguration Configure(Action<IServiceCollection> configure)
        {
            configure(ServiceCollection);
            return this;
        }

        public ServicesConfiguration AddService<TInterface, TType>(Func<TType> factory, Action<TType>? asser = null)
            where TType : TInterface
            where TInterface : class
        {
            ServiceEntries.Add(new GenericServiceEntry<TInterface, TType>(factory()) {Asseration = asser});
            return this;
        }

        public ServicesConfiguration AddService<TInterface, TType>(Action<TType>? asser = null)
            where TType : TInterface, new()
            where TInterface : class
        {
            return AddService<TInterface, TType>(() => new TType(), asser);
        }

        public ServicesConfiguration AddService<TType>(Func<TType> factory, Action<TType>? asser = null)
            where TType : class
        {
            ServiceEntries.Add(new SingleGenericServiceEntry<TType>(factory()) { Asseration = asser });
            return this;
        }

        public ServicesConfiguration AddService<TType>(Action<TType>? asser = null)
            where TType : class, new()
        {
            return AddService(() => new TType(), asser);
        }
    }
}