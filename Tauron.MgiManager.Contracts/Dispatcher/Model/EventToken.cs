using System;

namespace Tauron.MgiProjectManager.Dispatcher.Model
{
    public abstract class EventToken
    {
        public abstract Type HubType { get; }

        protected EventToken()
        {
        }
    }
}