using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CQRSlite.Events;
using CQRSlite.Messages;
using CQRSlite.Queries;
using Tauron.CQRS.Common.ServerHubs;

namespace Tauron.TestHelper.Mocks
{
    public class MockDispatcher : ITestDispatcher
    {
        private readonly Action<string, Func<IMessage, ServerDomainMessage, CancellationToken, Task>> _addHandler;
        private readonly Func<object, object> _query;
        private readonly Action<string, ServerDomainMessage, CancellationToken> _sendToClient;
        private readonly Action<IMessage, CancellationToken> _send;

        public MockDispatcher(Action<string, Func<IMessage, ServerDomainMessage, CancellationToken, Task>> addHandler = null, 
            Func<object, object> query = null, 
            Action<string, ServerDomainMessage, CancellationToken> sendToClient = null,
            Action<IMessage, CancellationToken> send = null)
        {
            _addHandler = addHandler;
            _query = query;
            _sendToClient = sendToClient;
            _send = send;
        }

        public Task Start(CancellationToken token)
        {
            return Task.CompletedTask;
        }

        public Task Stop()
        {
            return Task.CompletedTask;
        }

        public Task Send(IMessage command, CancellationToken cancellationToken)
        {
            _send?.Invoke(command, cancellationToken);
            return Task.CompletedTask;
        }

        public Task SendToClient(string client, ServerDomainMessage serverDomainMessage, CancellationToken token)
        {
            _sendToClient?.Invoke(client, serverDomainMessage, token);

            return Task.CompletedTask;
        }

        public async Task SendEvents(IEnumerable<IEvent> events, CancellationToken cancellationToken)
        {
            foreach (var @event in events) 
                await Send(@event, cancellationToken);
        }

        public Task Subscribe(string name, Func<IMessage, ServerDomainMessage, CancellationToken, Task> msg)
        {
            _addHandler?.Invoke(name, msg);
            return Task.CompletedTask;
        }

        public Task<TResponse> Query<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken)
        {
            var res = _query?.Invoke(query);
            if (res is TResponse response)
                return Task.FromResult(response);
            return Task.FromResult(default(TResponse));
        }

        public void AddHandler(string name, Func<IMessage, ServerDomainMessage, CancellationToken, Task> handler)
        {
            _addHandler?.Invoke(name, handler);
        }
    }
}