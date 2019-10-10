using System;
using CQRSlite.Events;
using Tauron.CQRS.Services.Core;

namespace Tauron.CQRS.Services
{
    public abstract class BaseEvent : IEvent
    {
        public virtual Guid Id { get; set; } = IdGenerator.Generator.NewGuid();

        public virtual int Version { get; set; } = 0;

        public virtual DateTimeOffset TimeStamp { get; set; } = DateTimeOffset.Now;
    }
}