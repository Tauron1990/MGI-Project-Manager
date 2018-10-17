using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CQRSlite.Events;
using CQRSlite.Messages;
using CQRSlite.Queries;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Tauron.CQRS.Common.ServerHubs;
using Tauron.CQRS.Services.Core.Components;
using Tauron.CQRS.Services.Extensions;

namespace Tauron.TestHelper.Core
{
    public class TestDispatcher : ITestDispatcher
    {
        private readonly IServiceProvider _serviceProvider;

        public ConcurrentDictionary<string, Func<IMessage, ServerDomainMessage, CancellationToken, Task>> Bus { get; } 
            = new ConcurrentDictionary<string, Func<IMessage, ServerDomainMessage, CancellationToken, Task>>();

        public TestDispatcher(IServiceProvider serviceProvider) 
            => _serviceProvider = serviceProvider;

        public Task Start(CancellationToken token) => Task.CompletedTask;

        public Task Stop() => Task.CompletedTask;

        public async Task Send(IMessage command, CancellationToken cancellationToken)
        {
            var smsg = command.ToDomainMessage();

            if (Bus.TryGetValue(smsg.EventName, out var handler))
                await handler(command, smsg, cancellationToken);
        }

        public async Task SendToClient(string client, ServerDomainMessage serverDomainMessage, CancellationToken token)
        {
            var msg = (IMessage) JsonConvert.DeserializeObject(serverDomainMessage.EventData, Type.GetType(serverDomainMessage.TypeName));

            if (Bus.TryGetValue(serverDomainMessage.EventName, out var handler))
                await handler(msg, serverDomainMessage, token);

        }

        public async Task SendEvents(IEnumerable<IEvent> events, CancellationToken cancellationToken)
        {
            foreach (var @event in events) 
                await Send(@event, cancellationToken);
        }

        public Task Subscribe(string name, Func<IMessage, ServerDomainMessage, CancellationToken, Task> msg)
        {
            Bus[name] = msg;
            return Task.CompletedTask;
        }

        public async Task<TResponse> Query<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken)
        {
            return await _serviceProvider.GetRequiredService<QueryAwaiter<TResponse>>().SendQuery(query, cancellationToken, async query1 =>
            {
                var smsg = query1.ToDomainMessage(true);

                if (Bus.TryGetValue(smsg.EventName, out var handler))
                    await handler(query1, smsg, cancellationToken);
            });
        }

        public void AddHandler(string name, Func<IMessage, ServerDomainMessage, CancellationToken, Task> handler)
        {
            if(Bus.ContainsKey(name)) return;

            Bus[name] = handler;
        }
    }
}