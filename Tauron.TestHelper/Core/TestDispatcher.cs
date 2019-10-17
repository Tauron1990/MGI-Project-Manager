using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CQRSlite.Events;
using CQRSlite.Messages;
using CQRSlite.Queries;
using Tauron.CQRS.Common.ServerHubs;
using Tauron.CQRS.Services;

namespace Tauron.TestHelper.Core
{
    public class TestDispatcher : IDispatcherClient, ITestDispatcher
    {
        public ConcurrentDictionary<string, Func<IMessage, ServerDomainMessage, CancellationToken, Task>> Bus { get; } 
            = new ConcurrentDictionary<string, Func<IMessage, ServerDomainMessage, CancellationToken, Task>>();

        public Task Start(CancellationToken token) => Task.CompletedTask;

        public Task Stop() => Task.CompletedTask;

        public async Task Send(IMessage command, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task SendToClient(string client, ServerDomainMessage serverDomainMessage, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public async Task SendEvents(IEnumerable<IEvent> events, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task Subscribe(string name, Func<IMessage, ServerDomainMessage, CancellationToken, Task> msg)
        {
            throw new NotImplementedException();
        }

        public async Task<TResponse> Query<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}