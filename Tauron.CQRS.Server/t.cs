using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;

namespace Tauron.CQRS.Server
{
    public class HeartBeat : BackgroundService
    {
        private readonly IHubContext<SignalRHub> _hubContext; //TODO Correct and Register Heartbead Service
        public HeartBeat(IHubContext<SignalRHub> hubContext)
        {
            _hubContext = hubContext;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await _hubContext.Clients.All.SendAsync("Heartbeat", DateTime.Now, stoppingToken);
                await Task.Delay(30000, stoppingToken);
            }
        }
    }
}