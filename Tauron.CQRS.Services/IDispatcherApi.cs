using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Tauron.CQRS.Common.ServerHubs;

namespace Tauron.CQRS.Services
{
    public interface IDispatcherApi
    {
        Task Save(IEnumerable<ServerDomainMessage> events);

        Task<IEnumerable<ServerDomainMessage>> Get(Guid aggregateId, int fromVersion, CancellationToken cancellationToken = new CancellationToken());
    }
}