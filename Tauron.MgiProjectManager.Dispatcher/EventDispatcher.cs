using System;
using System.Collections.Generic;
using System.Linq;
using Tauron.MgiProjectManager.Dispatcher.Model;

namespace Tauron.MgiProjectManager.Dispatcher
{
    [Export(typeof(IEventDispatcher), LiveCycle = LiveCycle.Singleton)]
    public class EventDispatcher : IEventDispatcher
    {
        private readonly Dictionary<Type, EventToken> _eventTokens;

        public EventDispatcher(IEnumerable<IEventHelper> helper) 
            => _eventTokens = helper.Select(h => h.GetEventToken()).ToDictionary(t => t.EventType);

        public TypedEventToken<TEvent> GetToken<TEvent>() 
            => _eventTokens[typeof(TEvent)] as TypedEventToken<TEvent>;
    }
}