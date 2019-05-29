using System;
using CQRSlite.Events;

namespace Tauron.CQRS.Common.Dto
{
    public class EventFailedEventMessage : IEvent
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public int Version { get; set; } = -1;
        public DateTimeOffset TimeStamp { get; set; } = DateTimeOffset.Now;

        public string EventName { get; set; }
    }
}