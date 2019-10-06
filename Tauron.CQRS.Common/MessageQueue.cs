using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Tauron.CQRS.Common
{
    public sealed class MessageQueue<TMessage> : IDisposable
    {
        private readonly bool _skipExceptions;
        private readonly BlockingCollection<TMessage> _incomming;
        private readonly BlockingCollection<Task> _processorQueue;

        public event Func<Exception, Task> OnError;

        public event Func<TMessage, Task> OnWork;

        private Task _dispatcher;
        private bool _stop;

        public MessageQueue(int maxParallel = int.MaxValue, bool skipExceptions = true)
        {
            _skipExceptions = skipExceptions;

            _incomming = new BlockingCollection<TMessage>();
            _processorQueue = new BlockingCollection<Task>(maxParallel);
        }

        public void Enqueue(TMessage msg) => _incomming.Add(msg);

        public async Task Start()
        {
            if(_stop) 
                throw new InvalidOperationException("Message Queue Stoped");
            if (_dispatcher != null)
                throw new InvalidOperationException("Message Queue started");

            _dispatcher = Task.Run(async () =>
            {
                foreach (var message in _incomming.GetConsumingEnumerable())
                {
                    try
                    {
                        var worker = OnWork;
                        if (worker == null) continue;

                        _processorQueue.Add(Task.Run(async () => await worker(message)));
                    }
                    catch (Exception e)
                    {
                        await ProcessError(e);

                        if (_skipExceptions) continue;

                        throw;
                    }
                }
            }).ContinueWith(t => _processorQueue.CompleteAdding());

            foreach (var task in _processorQueue.GetConsumingEnumerable())
            {
                try
                {
                    await task;
                }
                catch (Exception e)
                {
                    await ProcessError(e);

                    if (_skipExceptions) continue;

                    throw;
                }
            }

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

        private async Task ProcessError(Exception e)
        {
            var invoker = OnError;

            if (invoker != null)
                await invoker(e);
        }
    }
}