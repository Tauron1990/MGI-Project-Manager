using System;

namespace Tauron.MgiProjectManager.Dispatcher.Model
{
    public abstract class TypedEventToken<TEvent> : EventToken
    {
        public override Type HubType => typeof(TEvent);

        public abstract TEvent Hub { get; }


        public static implicit operator TEvent(TypedEventToken<TEvent> token) => token.Hub;
    }
}