using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Neleus.DependencyInjection.Extensions;

namespace Tauron.Application.TauronHost
{
    public sealed class CommonLifetime : IHostLifetime
    {
        private readonly IServiceByNameFactory<IAppRoute> _factory;
        private readonly ILogger<CommonLifetime> _logger;
        private readonly string _appRoute;

        private IAppRoute? _route;

        public CommonLifetime(IConfiguration configuration, IServiceByNameFactory<IAppRoute> factory, ILogger<CommonLifetime> logger)
        {
            _factory = factory;
            _logger = logger;
            _appRoute = configuration.GetValue("route", string.Empty);
        }

        public async Task WaitForStartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Begin Start Application");
            try
            {
                string name = !string.IsNullOrEmpty(_appRoute) ? _appRoute : "default";
                _logger.LogInformation("Try get Route for {RouteName}", name);

                _route = _factory.GetByName(name);
            }
            catch (SystemException e)
            {
                _logger.LogWarning(e, "Error on get Route");
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