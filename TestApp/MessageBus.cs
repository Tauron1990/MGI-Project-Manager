using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Auto_Fan_Control.Bus
{
    public sealed class MessageBus : IAsyncDisposable
    {
        private readonly object _lock = new object();
        private readonly ConcurrentDictionary<Type, List<object>> _handlers = new ConcurrentDictionary<Type, List<object>>();

        public IAsyncDisposable Subscribe<TMsg>(IHandler<TMsg> handler)
        {
            var msgType = typeof(TMsg);
            lock (_lock)
            {
                if (_handlers.TryGetValue(msgType, out var list))
                {
                    list.Add(handler);
                }
                else
                    _handlers.TryAdd(msgType, new List<object>() {handler});
            }

            return new SubscribeDispose<TMsg>(handler, _handlers[msgType], _lock);
        }

        public async Task Publish<TMsg>(TMsg msg)
        {
            foreach (var handler in _handlers[typeof(TMsg)].OfType<IHandler<TMsg>>()) await handler.Handle(msg, this).ConfigureAwait(false);
        }

        public async ValueTask DisposeAsync()
        {
            foreach (var dis in _handlers.SelectMany(l => l.Value))
            {
                switch (dis)
                {
                    case IAsyncDisposable asyncDisposable:
                        await asyncDisposable.DisposeAsync();
                        break;
                    case IDisposable disposable:
                        disposable.Dispose();
                        break;
                }
            }

            _handlers.Clear();
        }

        private sealed class SubscribeDispose<TMsg> : IAsyncDisposable
        {
            private readonly IHandler<TMsg> _handler;
            private readonly List<object> _handlers;
            private readonly object _locker;

            public SubscribeDispose(IHandler<TMsg> handler, List<object> handlers, object locker)
            {
                _handler = handler;
                _handlers = handlers;
                _locker = locker;
            }

            public ValueTask DisposeAsync()
            {
                lock (_locker)
                    _handlers.Remove(_handler);
                return new ValueTask(Task.CompletedTask);
            }
        }
    }
}