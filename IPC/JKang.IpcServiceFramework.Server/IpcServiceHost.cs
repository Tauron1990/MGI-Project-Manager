using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace JKang.IpcServiceFramework
{
    public class IpcServiceHost : IIpcServiceHost
    {
        private readonly List<IpcServiceEndpoint> _endpoints;
        private readonly ILogger<IpcServiceHost> _logger;

        public IpcServiceHost(IEnumerable<IpcServiceEndpoint> endpoints, IServiceProvider serviceProvider)
        {
            _endpoints = endpoints.ToList();
            _logger = serviceProvider.GetService<ILogger<IpcServiceHost>>();
        }

        public void Run()
        {
            RunAsync().Wait();
        }

        public Task RunAsync(CancellationToken cancellationToken = default)
        {
            var tasks = _endpoints
               .Select(endpoint =>
                {
                    _logger.LogDebug($"Starting endpoint '{endpoint.Name}'...");

                    cancellationToken.Register(() => { _logger.LogDebug($"Stopping endpoint '{endpoint.Name}'..."); });

                    return endpoint.ListenAsync(cancellationToken).ContinueWith(_ => { _logger.LogDebug($"Endpoint '{endpoint.Name}' stopped."); }, cancellationToken);
                })
               .ToArray();
            return Task.WhenAll(tasks);
        }
    }
}