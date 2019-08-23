using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CQRSlite.Events;
using JetBrains.Annotations;

namespace Tauron.CQRS.Services.Core.Components
{
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedWithFixedConstructorSignature)]
    public sealed class GlobalEventHandler<TMessage> : IEventHandler<TMessage> where TMessage : IEvent
    {
        private class RegisterDispose : IDisposable
        {
            private readonly GlobalEventHandler<TMessage> _handler;
            private readonly object _key;

            public RegisterDispose(GlobalEventHandler<TMessage> handler, object key)
            {
                _handler = handler;
                _key = key;
            }

            public void Dispose() => _handler._handlerRegistry.Remove(_key);
        }

        private readonly Dictionary<object, Func<TMessage, Task>> _handlerRegistry = new Dictionary<object, Func<TMessage, Task>>();

        public IDisposable Register(Func<TMessage, Task> awaiter)
        {
            var key = new object();

            _handlerRegistry[key] = awaiter;

            return new RegisterDispose(this, key);
        }

        public async Task Handle(TMessage message)
        {
            foreach (var handlerRegistryValue in _handlerRegistry.Values) await handlerRegistryValue(message);
        }
    }
}