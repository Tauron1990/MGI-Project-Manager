using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CQRSlite.Events;
using CQRSlite.Messages;
using CQRSlite.Queries;
using Tauron.CQRS.Common.ServerHubs;

namespace Tauron.CQRS.Services
{
    public interface IDispatcherClient
    {
        Task Start(CancellationToken token);
        
        Task Stop();
        
        Task Send(IMessage command, CancellationToken cancellationToken);

        Task SendToClient(string client, ServerDomainMessage serverDomainMessage, CancellationToken token);

        Task SendEvents(IEnumerable<IEvent> events, CancellationToken cancellationToken);

        Task Subscribe(string name, Func<IMessage, ServerDomainMessage, CancellationToken, Task> msg);

        Task<TResponse> Query<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken);
    }
}