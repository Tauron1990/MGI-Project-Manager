using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace Tauron.Application
{
    [PublicAPI]
    public abstract class SharedEvent<TPayload>
    {
        private readonly WeakActionEvent<TPayload> _handlerList = new WeakActionEvent<TPayload>();
        
        public virtual void Publish(TPayload content) => _handlerList.Invoke(content);

        public void Subscribe(Action<TPayload> handler) => _handlerList.Add(Argument.NotNull(handler, nameof(handler)));

        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Un")]
        public void UnSubscribe(Action<TPayload> handler) => _handlerList.Remove(Argument.NotNull(handler, nameof(handler)));
        
    }

    [PublicAPI]
    public interface IEventAggregator
    {
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        TEventType GetEvent<TEventType, TPayload>() where TEventType : SharedEvent<TPayload>, new();
    }

    [PublicAPI]
    public sealed class EventAggregator : IEventAggregator
    {
        private readonly Dictionary<Type, object> _events = new Dictionary<Type, object>();
        
        public TEventType GetEvent<TEventType, TPayload>() where TEventType : SharedEvent<TPayload>, new()
        {
            var t = typeof(TEventType);
            if (!_events.ContainsKey(t)) _events[t] = new TEventType();

            return (TEventType) _events[t];
        }
    }
}