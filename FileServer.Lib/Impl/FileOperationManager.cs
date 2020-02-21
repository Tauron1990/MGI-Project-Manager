using System;
using System.Collections.Concurrent;
using System.Threading;

namespace FileServer.Lib.Impl
{
    public sealed class FileOperationManager : IFileOperationManager, IDisposable
    {
        private readonly Timer _checkTimer;
        private readonly ConcurrentDictionary<Guid, OperationEntry> _operations = new ConcurrentDictionary<Guid, OperationEntry>();

        public FileOperationManager() 
            => _checkTimer = new Timer(CheckExpiration, null, TimeSpan.FromSeconds(60), TimeSpan.FromSeconds(60));

        public void Dispose() 
            => _checkTimer.Dispose();

        public IKeeper<TOp>? Get<TOp>(Guid id) where TOp : class, IInternalOperation
        {
            return _operations.TryGetValue(id, out var op) ? KeeperImpl<TOp>.TryCreate(op.Operation) : null;
        }

        public bool Set<TOp>(TOp op, Guid id) where TOp : class, IInternalOperation 
            => _operations.TryAdd(id, new OperationEntry(op, DateTime.Now + op.Timeout));

        public void Refresh(Guid id)
        {
            if (!_operations.TryGetValue(id, out var op)) return;
            
            lock (op.Operation)
            {
                if(op.Operation.IsCollected)
                    return;

                op.Expiration = DateTime.Now + op.Operation.Timeout;
            }
        }

        private void CheckExpiration(object state)
        {
            foreach (var (key, entry) in _operations.ToArray())
            {
                var realOp = entry.Operation;
                var cd = DateTime.Now;

                if(!Monitor.TryEnter(realOp))
                    continue;

                try
                {
                    if (realOp.IsCollected)
                    {
                        _operations.TryRemove(key, out _);
                        continue;
                    }
                    
                    if(entry.Expiration > cd)
                        continue;

                    realOp.Dispose();
                    _operations.TryRemove(key, out _);
                }
                finally
                {
                    Monitor.Exit(realOp);
                }
            }
        }

        private sealed class OperationEntry
        {
            public IInternalOperation Operation { get; }

            public DateTime Expiration { get; set; }

            public OperationEntry(IInternalOperation operation, DateTime expiration)
            {
                Operation = operation;
                Expiration = expiration;
            }
        }

        private sealed class KeeperImpl<TType> : IKeeper<TType> where TType : class, IInternalOperation
        {
            public static IKeeper<TType>? TryCreate(IInternalOperation operation)
            {
                Monitor.Enter(operation);

                if (operation.IsCollected)
                    return null;
                
                if(operation is TType target)
                    return new KeeperImpl<TType>(target);

                return null;
            }

            public void Dispose() 
                => Monitor.Exit(Operation);

            public TType Operation { get; }

            private KeeperImpl(TType operation) 
                => Operation = operation;
        }
    }
}