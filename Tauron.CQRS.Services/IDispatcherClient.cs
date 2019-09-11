using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CQRSlite.Events;
using CQRSlite.Messages;
using CQRSlite.Queries;

namespace Tauron.CQRS.Services
{
    public interface IDispatcherClient
    {
        Task Start(CancellationToken token);
        Task Stop();
        
        Task Send(IMessage command, CancellationToken cancellationToken);

        Task SendEvents(IEnumerable<IEvent> events, CancellationToken cancellationToken);

        Task Subsribe(string name, Func<IMessage, CancellationToken, Task> msg, bool isCommand);

        Task<TResponse> Query<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken);
    }
}