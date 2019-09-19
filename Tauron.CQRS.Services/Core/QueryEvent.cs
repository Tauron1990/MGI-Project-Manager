using System;
using CQRSlite.Events;
using Newtonsoft.Json.Linq;

namespace Tauron.CQRS.Services.Core
{
    public class QueryEvent<TResponse> : IEvent
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public int Version { get; set; } = -1;
        public DateTimeOffset TimeStamp { get; set; } = DateTimeOffset.Now;

        public string EventName { get; set; }

        public TResponse Data { get; set; }

        public QueryEvent()
        {
            
        }

        public QueryEvent(string eventName, TResponse data)
        {
            EventName = eventName;
            Data = data;
        }
    }
}