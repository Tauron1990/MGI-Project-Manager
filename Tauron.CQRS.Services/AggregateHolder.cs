using System;
using System.Threading.Tasks;
using CQRSlite.Domain;
using Nito.AsyncEx;

namespace Tauron.CQRS.Services
{
    public abstract class AggregateHolderBase
    {
        internal AggregateRoot Aggregate { get; private set; }
    }

    public sealed class AggregateHolder<TType> : AggregateHolderBase
        where TType : AggregateRoot
    {
        private readonly TType _aggregate;
        private readonly AsyncReaderWriterLock _readerWriterLock = new AsyncReaderWriterLock();

        public AggregateHolder(TType aggregate) => _aggregate = aggregate;

        public async Task Read(Func<TType, Task> reader)
        {
            using (await _readerWriterLock.ReaderLockAsync()) await reader(_aggregate);
        }

        public async Task Write(Func<TType, Task> writer)
        {
            using (await _readerWriterLock.WriterLockAsync()) await writer(_aggregate);
        }
    }
}