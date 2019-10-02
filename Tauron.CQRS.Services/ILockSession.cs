using System;
using System.Threading;
using System.Threading.Tasks;
using CQRSlite.Domain;

namespace Tauron.CQRS.Services
{
    public interface ILockSession
    {
        Task Add<T>(T aggregate, CancellationToken cancellationToken = default(CancellationToken)) where T : AggregateRoot;

        Task<AggregateHolder<T>> Get<T>(Guid id, int? expectedVersion = null, CancellationToken cancellationToken = default(CancellationToken)) where T : AggregateRoot;

        Task Commit(CancellationToken cancellationToken = default(CancellationToken));
    }
}