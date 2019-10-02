using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Tauron.CQRS.Common
{
    public sealed class MessageQueue<TMessage> : IDisposable
    {
        private readonly bool _skipExceptions;
        private readonly BlockingCollection<TMessage> _incomming;
        private readonly BlockingCollection<TMessage> _processorQueue;

        public event Func<Exception, Task> OnError;

        public event Func<TMessage, Task> OnWork;

        private Task _dispatcher;
        private bool _stop;

        public MessageQueue(int maxParallel = int.MaxValue, bool skipExceptions = true)
        {
            _skipExceptions = skipExceptions;

            _incomming = new BlockingCollection<TMessage>();
            _processorQueue = new BlockingCollection<TMessage>(maxParallel);
        }

        public void Enqueue(TMessage msg) => _incomming.Add(msg);

        public async Task Start()
        {
            if(_stop) 
                throw new InvalidOperationException("Message Queue Stoped");
            if (_dispatcher != null)
                throw new InvalidOperationException("Message Queue started");

            _dispatcher = Task.Run(() =>
            {
                foreach (var message in _incomming.GetConsumingEnumerable())
                    _processorQueue.Add(message);
            });

            foreach (var message in _processorQueue.GetConsumingEnumerable())
            {
                try
                {
                    var worker = OnWork;
                    if(worker == null) continue;

                    await worker(message);
                }
                catch (Exception e)
                {
                    var invoker = OnError;

                    if (invoker != null)
                        await invoker(e);

                    if(_skipExceptions) continue;

                    throw;
                }
            }

            _processorQueue.CompleteAdding();
            OnError = null;
            OnWork = null;
        }

        public void Stop()
        {
            lock (this)
            {
                if(_stop) return;
                _stop = true;
                _incomming.CompleteAdding();
            }
        }

        public void Dispose()
        {
            _incomming.Dispose();
            _processorQueue.Dispose();
            _dispatcher?.Dispose();
        }
    }
}