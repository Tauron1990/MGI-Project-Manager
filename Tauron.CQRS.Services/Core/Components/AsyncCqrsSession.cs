using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CQRSlite.Domain;
using CQRSlite.Domain.Exception;
using CQRSlite.Events;

namespace Tauron.CQRS.Services.Core.Components
{
    public class AsyncCqrsSession : ILockSession
    {
        private readonly IDispatcherClient _eventPublisher;
        private readonly IRepository _repository;
        private readonly ConcurrentDictionary<Guid, AggregateDescriptor> _trackedAggregates;

        public AsyncCqrsSession(IRepository repository, IDispatcherClient eventPublisher)
        {
            _eventPublisher = eventPublisher;
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _trackedAggregates = new ConcurrentDictionary<Guid, AggregateDescriptor>();
        }

        public Task Add<T>(T aggregate, CancellationToken cancellationToken = default) where T : AggregateRoot
        {
            if (!IsTracked(aggregate.Id))
            {
                _trackedAggregates.TryAdd(aggregate.Id, new AggregateDescriptor()
                {
                    AggregateRoot = new AggregateHolder<T>(aggregate),
                    Version = aggregate.Version
                });
            }
            else if (_trackedAggregates[aggregate.Id].AggregateRoot.Aggregate != aggregate)
                throw new ConcurrencyException(aggregate.Id);
            return Task.FromResult(0);
        }

        public async Task<AggregateHolder<TAggregateRoot>> Get<TAggregateRoot>(
            Guid id,
            int? expectedVersion = null,
            CancellationToken cancellationToken = default)
            where TAggregateRoot : AggregateRoot
        {
            if (IsTracked(id))
            {
                var aggregate1 = _trackedAggregates[id].AggregateRoot;
                if (!expectedVersion.HasValue) return (AggregateHolder<TAggregateRoot>) aggregate1;
                var version = aggregate1.Aggregate.Version;
                var nullable = expectedVersion;
                var valueOrDefault = nullable.GetValueOrDefault();
                if (version != valueOrDefault)
                    throw new ConcurrencyException(aggregate1.Aggregate.Id);

                return (AggregateHolder<TAggregateRoot>) aggregate1;
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
            return  (AggregateHolder<TAggregateRoot>) _trackedAggregates[aggregate.Id].AggregateRoot;
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
                    events.AddRange(aggregateDescriptor.AggregateRoot.Aggregate.GetUncommittedChanges());
                    taskArray[index] = _repository.Save(aggregateDescriptor.AggregateRoot.Aggregate, aggregateDescriptor.Version, cancellationToken);
                    ++index;
                }

                await Task.WhenAll(taskArray).ConfigureAwait(false);
                await _eventPublisher.SendEvents(events, cancellationToken);
            }
            finally
            {
                _trackedAggregates.Clear();
            }
        }

        private class AggregateDescriptor
        {
            public AggregateHolderBase AggregateRoot { get; set; }

            public int Version { get; set; }
        }
    }
}