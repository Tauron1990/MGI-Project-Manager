using System;

namespace Tauron.MgiProjectManager.Dispatcher.Model
{
    public abstract class TypedEventToken<TEvent> : EventToken
    {
        public override Type EventType => typeof(TEvent);

        public abstract TEvent EventElement { get; }


        public static implicit operator TEvent(TypedEventToken<TEvent> token) => token.EventElement;
    }
}