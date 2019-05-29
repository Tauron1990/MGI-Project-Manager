using System;
using System.Threading;
using System.Threading.Tasks;
using CQRSlite.Domain;

namespace Tauron.CQRS.Services.Core
{
    public class ServerRepository : IRepository
    {
        public Task Save<T>(T aggregate, int? expectedVersion = null, CancellationToken cancellationToken = new CancellationToken()) where T : AggregateRoot
        {
            
        }

        public Task<T> Get<T>(Guid aggregateId, CancellationToken cancellationToken = new CancellationToken()) where T : AggregateRoot
        {

        }
    }
}