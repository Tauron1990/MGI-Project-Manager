using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Tauron.CQRS.Common.ServerHubs;

namespace Tauron.CQRS.Services
{
    public interface IDispatcherApi
    {
        Task Save(IEnumerable<DomainMessage> events);

        Task<IEnumerable<DomainMessage>> Get(Guid aggregateId, int fromVersion, CancellationToken cancellationToken = new CancellationToken());
    }
}