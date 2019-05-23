using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Tauron.CQRS.Common.ServerHubs;

namespace Tauron.CQRS.Services.Core
{
    public class DispatcherApi : IDispatcherApi
    {
        public Task Save(IEnumerable<DomainEvent> events, CancellationToken cancellationToken = new CancellationToken())
        {
            
        }

        public Task<IEnumerable<DomainEvent>> Get(Guid aggregateId, int fromVersion, CancellationToken cancellationToken = new CancellationToken())
        {
            
        }
    }
}