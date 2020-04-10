using System;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;

namespace JKang.IpcServiceFramework
{
    [PublicAPI]
    public interface IIpcServiceBuilder
    {
        IServiceCollection Services { get; }

        IIpcServiceBuilder AddService<TInterface, TImplementation>()
            where TInterface : class
            where TImplementation : class, TInterface;

        IIpcServiceBuilder AddService<TInterface, TImplementation>(Func<IServiceProvider, TImplementation> implementationFactory)
            where TInterface : class
            where TImplementation : class, TInterface;

        IIpcServiceBuilder AddService<TInterface>(Func<IServiceProvider, TInterface> implementationFactory)
            where TInterface : class;
    }
}