using System;
using CQRSlite.Events;

namespace Tauron.CQRS.Services.Core
{
    public class QueryEvent : IEvent
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public int Version { get; set; } = -1;
        public DateTimeOffset TimeStamp { get; set; } = DateTimeOffset.Now;
    }
}