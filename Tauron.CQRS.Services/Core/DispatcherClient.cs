using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Tauron.CQRS.Common.Configuration;

namespace Tauron.CQRS.Services.Core
{
    public class DispatcherClient : IDispatcherClient
    {
        private readonly IOptions<ClientCofiguration> _config;
        private readonly HubConnection _hubConnection;

        private bool _isCLoseOk;

        public DispatcherClient(IOptions<ClientCofiguration> config)
        {
            _config = config;

            _hubConnection = new HubConnectionBuilder().AddJsonProtocol().WithUrl(config.Value.EventHubUrl).Build();
        }

        public async Task Start(CancellationToken token)
        {
            await _hubConnection.StartAsync(token);

            _hubConnection.Closed += HubConnectionOnClosed;
        }

        private async Task HubConnectionOnClosed(Exception arg)
        {
            if(_isCLoseOk) return;

            await _hubConnection.StartAsync();
        }
    }
}