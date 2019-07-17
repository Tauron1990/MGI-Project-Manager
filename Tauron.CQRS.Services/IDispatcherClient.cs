using System;
using System.Threading;
using System.Threading.Tasks;
using CQRSlite.Messages;

namespace Tauron.CQRS.Services
{
    public interface IDispatcherClient
    {
        Task Start(CancellationToken token);
        Task Stop();
        Task Send(IMessage command, CancellationToken cancellationToken);

        Task Subsribe(string name, Func<IMessage, CancellationToken, Task> msg, bool isCommand);
    }
}