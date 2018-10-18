using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CQRSlite.Domain;
using CQRSlite.Domain.Exception;
using CQRSlite.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using SequentialGuid;
using Tauron.CQRS.Services.Core.Components;
using Tauron.TestHelper.Data;

namespace Tauron.TestHelper.Mocks
{
    public class MockSession : ICqrsSession
    {
        private readonly Dictionary<Guid, AggregateRoot> _database;
        private readonly Action<object> _add;
        private readonly Func<Guid, int?, AggregateRoot> _get;
        private readonly Action _commit;

        public MockSession(Dictionary<Guid, AggregateRoot> database, IEventPublisher eventPublisher = null, Action<object> add = null, Func<Guid, int?, AggregateRoot> get = null, Action commit = null)
        {
            EventPublisher = eventPublisher;
            _database = database;
            _add = add;
            _get = get;
            _commit = commit;
        }

        public Task Add<T>(T aggregate, CancellationToken cancellationToken = new CancellationToken()) where T : AggregateRoot
        {
            _add?.Invoke(aggregate);
            _database[aggregate.Id] = aggregate;

            return Task.CompletedTask;
        }

        public Task<T> Get<T>(Guid id, int? expectedVersion = null, CancellationToken cancellationToken = new CancellationToken()) where T : AggregateRoot
        {
            var agg = _get?.Invoke(id, expectedVersion);
            if (agg is T a) return Task.FromResult(a);
            if (_database.TryGetValue(id, out agg) && agg is T aa) return Task.FromResult(aa);

            throw new AggregateNotFoundException(typeof(T), id);
        }

        public Task Commit(CancellationToken cancellationToken = new CancellationToken())
        {
            _commit?.Invoke();
            return Task.CompletedTask;
        }

        public IEventPublisher EventPublisher { get; }
    }
}