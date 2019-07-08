using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Tauron.CQRS.Health.Core.Impl
{
    public class Buffer<T> : ConcurrentQueue<T>
    {
        private int? MaxCapacity { get; }

        public Buffer(int capacity) { MaxCapacity = capacity; }

        public void Add(T newElement)
        {
            if (Count == (MaxCapacity ?? -1)) TryDequeue(out _); // no limit if maxCapacity = null
            Enqueue(newElement);
        }
    }
}