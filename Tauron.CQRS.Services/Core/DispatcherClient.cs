using System;
using System.Threading;
using System.Threading.Tasks;
using CQRSlite.Messages;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Tauron.CQRS.Common.Configuration;
using Tauron.CQRS.Common.ServerHubs;

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

        public async Task Stop()
        {
            _isCLoseOk = true;
            await _hubConnection.StopAsync();
            await _hubConnection.DisposeAsync();
        }

        private async Task HubConnectionOnClosed(Exception arg)
        {
            if(_isCLoseOk) return;

            await _hubConnection.StartAsync();
        }

        public async Task Send(IMessage command, CancellationToken cancellationToken)
        {
            await _hubConnection.SendAsync(HubEventNames.PublishEvent, new DomainMessage
            {
                EventData = command,
                EventName = command.GetType().Name,
                EventType = EventType.Command,
                SequenceNumber = -1
            }, _config.Value.ApiKey, cancellationToken: cancellationToken);
        }

        public Task Subsribe(string name, Func<IMessage, CancellationToken, Task> msg, bool isCommand)
        {
            _hubConnection.ResetSendPing();
        }
    }
}