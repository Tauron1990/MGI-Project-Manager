using System;
using CQRSlite.Events;
using Tauron.CQRS.Services.Core;

namespace Tauron.CQRS.Services
{
    public abstract class BaseEvent : IEvent
    {
        public abstract Guid Id { get; set; }

        public virtual int Version { get; set; } = 0;

        public virtual DateTimeOffset TimeStamp { get; set; } = DateTimeOffset.Now;
    }
}