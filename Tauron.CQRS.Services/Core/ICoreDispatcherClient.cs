using System;
using System.Threading;
using System.Threading.Tasks;
using CQRSlite.Messages;
using Tauron.CQRS.Common.ServerHubs;

namespace Tauron.CQRS.Services.Core
{
    public interface ICoreDispatcherClient : IDispatcherClient
    {
        void AddHandler(string name, Func<IMessage, ServerDomainMessage, CancellationToken, Task> handler);
    }
}