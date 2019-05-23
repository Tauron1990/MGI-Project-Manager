using System;
using CQRSlite.Events;

namespace Tauron.CQRS.Common.ServerHubs
{
    public class CQRSEvent : IEvent
    {
        public EventType EventType { get; set; }

        public Guid Id { get; set; }

        public int Version { get; set; }

        public DateTimeOffset TimeStamp { get; set; }

        public CQRSEvent(Guid id, int version)
        {
            Id = id;
            Version = version;
            TimeStamp = DateTimeOffset.Now;
        }
    }
}