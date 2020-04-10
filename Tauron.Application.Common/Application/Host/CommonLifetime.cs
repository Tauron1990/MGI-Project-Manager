using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Neleus.DependencyInjection.Extensions;

namespace Tauron.Application.Host
{
    public sealed class CommonLifetime : IHostLifetime
    {
        private readonly IServiceByNameFactory<IAppRoute> _factory;
        private readonly string _appRoute;

        private IAppRoute? _route;

        public CommonLifetime(IConfiguration configuration, IServiceByNameFactory<IAppRoute> factory)
        {
            _factory = factory;
            _appRoute = configuration.GetValue("route", string.Empty);
        }

        public async Task WaitForStartAsync(CancellationToken cancellationToken)
        {
            try
            {
                _route = _factory.GetByName(string.IsNullOrEmpty(_appRoute) ? _appRoute : "default");
            }
            catch (ArgumentException)
            {
                _route = _factory.GetByName("default");
            }

            await _route.WaitForStartAsync(cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if(_route == null)
                return;

            await _route.StopAsync(cancellationToken);
        }
    }
}