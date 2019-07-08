using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Tauron.CQRS.Common.ServerHubs;
using Tauron.CQRS.Server.Hubs;

namespace Tauron.CQRS.Server
{
    public class HeartBeatService : BackgroundService
    {
        private readonly IHubContext<EventHub> _hubContext;
        private readonly IConnectionManager _connectionManager;
        private readonly ILogger<HeartBeatService> _logger;

        public HeartBeatService(IHubContext<EventHub> hubContext, IConnectionManager connectionManager, ILogger<HeartBeatService> logger)
        {
            _hubContext = hubContext;
            _connectionManager = connectionManager;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Heartbeat");
                await _hubContext.Clients.All.SendAsync(HubEventNames.HeartbeatNames.Heartbeat, DateTime.Now, stoppingToken);
                await _connectionManager.UpdateAllConnection();
                await Task.Delay(30000, stoppingToken);
            }
        }
    }
}