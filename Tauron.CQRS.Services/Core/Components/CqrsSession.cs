using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CQRSlite.Domain;
using CQRSlite.Domain.Exception;
using CQRSlite.Events;

namespace Tauron.CQRS.Services.Core.Components
{
    public class CqrsSession : ISession
    {
        private readonly IDispatcherClient _dispatcherClient;
        private readonly IRepository _repository;
        private readonly Dictionary<Guid, AggregateDescriptor> _trackedAggregates;

        public IEventPublisher EventPublisher { get; }

        public CqrsSession(IRepository repository, IDispatcherClient dispatcherClient, IEventPublisher eventPublisher)
        {
            EventPublisher = eventPublisher;
            _dispatcherClient = dispatcherClient;
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _trackedAggregates = new Dictionary<Guid, AggregateDescriptor>();
        }

        public Task Add<T>(T aggregate, CancellationToken cancellationToken = default) where T : AggregateRoot
        {
            if (!IsTracked(aggregate.Id))
            {
                _trackedAggregates.Add(aggregate.Id, new AggregateDescriptor()
                                                     {
                                                         Aggregate =  aggregate,
                                                         Version = aggregate.Version
                                                     });
            }
            else if (_trackedAggregates[aggregate.Id].Aggregate != aggregate)
                throw new ConcurrencyException(aggregate.Id);
            return Task.CompletedTask;
        }

        public async Task<TAggregateRoot> Get<TAggregateRoot>(
            Guid id,
            int? expectedVersion = null,
            CancellationToken cancellationToken = default)
            where TAggregateRoot : AggregateRoot
        {
            if (IsTracked(id))
            {
                var aggregate1 = (TAggregateRoot) _trackedAggregates[id].Aggregate;
                if (!expectedVersion.HasValue) return aggregate1;
                var version = aggregate1.Version;
                var nullable = expectedVersion;
                var valueOrDefault = nullable.GetValueOrDefault();
                if (version != valueOrDefault)
                    throw new ConcurrencyException(aggregate1.Id);

                return aggregate1;
            }

            var aggregate = await _repository.Get<TAggregateRoot>(id, cancellationToken).ConfigureAwait(false);
            if (expectedVersion.HasValue)
            {
                var version = aggregate.Version;
                var nullable = expectedVersion;
                var valueOrDefault = nullable.GetValueOrDefault();
                if (version != valueOrDefault)
                    throw new ConcurrencyException(id);
            }

            await Add(aggregate, cancellationToken).ConfigureAwait(false);
            return aggregate;
        }

        private bool IsTracked(Guid id) => _trackedAggregates.ContainsKey(id);

        public async Task Commit(CancellationToken cancellationToken = default)
        {
            try
            {
                var events = new List<IEvent>();

                var taskArray = new Task[_trackedAggregates.Count];
                var index = 0;
                foreach (AggregateDescriptor aggregateDescriptor in _trackedAggregates.Values)
                {
                    events.AddRange(aggregateDescriptor.Aggregate.GetUncommittedChanges());
                    taskArray[index] = _repository.Save(aggregateDescriptor.Aggregate, aggregateDescriptor.Version, cancellationToken);
                    ++index;
                }

                await Task.WhenAll(taskArray).ConfigureAwait(false);
                await _dispatcherClient.SendEvents(events, cancellationToken);
            }
            finally
            {
                _trackedAggregates.Clear();
            }
        }

        private class AggregateDescriptor
        {
            public AggregateRoot Aggregate { get; set; }

            public int Version { get; set; }
        }
    }
}