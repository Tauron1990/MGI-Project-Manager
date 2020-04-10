using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Neleus.DependencyInjection.Extensions;
using Tauron.Application.Host;

namespace Tauron.Application.Wpf.AppCore
{
    public sealed class WpfConfiguration
    {
        internal readonly IServiceCollection ServiceCollection;
        private readonly ServicesByNameBuilder<IAppRoute, EmptyMeta> _services;

        public WpfConfiguration(IServiceCollection serviceCollection)
        {
            ServiceCollection = serviceCollection;
            serviceCollection.TryAddSingleton<IHostLifetime, CommonLifetime>();

            _services = serviceCollection.AddByName<IAppRoute>()
               .Add<AppLifetime>("default");
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