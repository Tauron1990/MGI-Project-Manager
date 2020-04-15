using System;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Neleus.DependencyInjection.Extensions;
using Tauron.Application.TauronHost;

namespace Tauron.Application.Wpf.AppCore
{
    [PublicAPI]
    public sealed class WpfConfiguration
    {
        internal readonly IServiceCollection ServiceCollection;
        private readonly ServicesByNameBuilder<IAppRoute, EmptyMeta> _services;

        public WpfConfiguration(IServiceCollection serviceCollection)
        {
            ServiceCollection = serviceCollection;

            serviceCollection.AddSingleton<IHostLifetime, CommonLifetime>();
            serviceCollection.AddSingleton<AppLifetime>();
            serviceCollection.AddSingleton<IWpfLifetime, WpfLifetime>();

            _services = serviceCollection.AddByName<IAppRoute>()
               .Add<AppLifetime>("default");
        }

        public WpfConfiguration WithAppFactory(Func<System.Windows.Application> factory)
        {
            ServiceCollection.TryAddTransient<IAppFactory>(sp => new DelegateAppFactory(factory));
            return this;
        }

        public WpfConfiguration WithRoute<TRoute>(string name)
            where TRoute : class, IAppRoute
        {
            ServiceCollection.AddSingleton<TRoute>();
            _services.Add<TRoute>(name);

            return this;
        }

        internal void Build() 
            => _services.Build();
    }
}