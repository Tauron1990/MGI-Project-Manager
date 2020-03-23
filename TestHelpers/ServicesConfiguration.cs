using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using TestHelpers.Core;

namespace TestHelpers
{
    public sealed class ServicesConfiguration
    {
        public IServiceCollection ServiceCollection { get; }

        internal List<ServiceEntry> ServiceEntries { get; } = new List<ServiceEntry>();

        public ServicesConfiguration(IServiceCollection serviceCollection)
        {
            ServiceCollection = serviceCollection;
        }

        public ServicesConfiguration AddService<TInterface, TType>(Func<TType> factory, Action<TType>? asser = null) 
            where TType : TInterface 
            where TInterface : class
        {
            ServiceEntries.Add(new GenericServiceEntry<TInterface, TType>(factory()) { Asseration = asser});
            return this;
        }

        public ServicesConfiguration AddService<TInterface, TType>( Action<TType>? asser = null)
            where TType : TInterface, new()
            where TInterface : class
        {
            return AddService<TInterface, TType>(() => new TType(), asser);
        }
    }
}