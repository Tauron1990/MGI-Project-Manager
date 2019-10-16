using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CQRSlite.Domain;
using CQRSlite.Domain.Exception;
using CQRSlite.Events;

namespace Tauron.CQRS.Services.Core.Components
{
    public class InternalRepository : IRepository
    {
        private readonly IEventStore _eventStore;

        public InternalRepository(IEventStore eventStore)
        {
            _eventStore = (eventStore ?? throw new ArgumentNullException(nameof(eventStore)));
        }

        public async Task Save<T>(T aggregate, int? expectedVersion = null, CancellationToken cancellationToken = default(CancellationToken)) where T : AggregateRoot
        {
            bool flag = expectedVersion.HasValue;
            if (flag)
            {
                flag = (await _eventStore.Get(aggregate.Id, expectedVersion.Value, cancellationToken).ConfigureAwait(continueOnCapturedContext: false)).Any();
            }
            if (flag)
            {
                throw new ConcurrencyException(aggregate.Id);
            }
            IEvent[] changes = aggregate.FlushUncommittedChanges();
            await _eventStore.Save(changes, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);

        }

        public Task<T> Get<T>(Guid aggregateId, CancellationToken cancellationToken = default(CancellationToken)) where T : AggregateRoot
        {
            return LoadAggregate<T>(aggregateId, cancellationToken);
        }

        private async Task<T> LoadAggregate<T>(Guid id, CancellationToken cancellationToken = default(CancellationToken)) where T : AggregateRoot
        {
            IEnumerable<IEvent> enumerable = await _eventStore.Get(id, -1, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
            if (!enumerable.Any())
            {
                throw new AggregateNotFoundException(typeof(T), id);
            }
            T val = AggregateFactory<T>.CreateAggregate();
            if(val is CoreAggregateRoot coreAggregate)
                coreAggregate.SetId(id);

            val.LoadFromHistory(enumerable);
            return val;
        }
    }
}