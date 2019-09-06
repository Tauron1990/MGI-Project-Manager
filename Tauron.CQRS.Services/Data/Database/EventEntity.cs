using System;
using System.ComponentModel.DataAnnotations;
using Tauron.CQRS.Common.ServerHubs;

namespace Tauron.CQRS.Services.Data.Database
{
    public class EventEntity
    {
        [Key]
        public long SequenceNumber { get; set; }

        public Guid? Id { get; set; }

        public EventType EventType { get; set; }

        public string Data { get; set; }

        public string EventName { get; set; }

        public string OriginType { get; set; }

        public int Version { get; set; }

        public DateTimeOffset TimeStamp { get; set; }
    }
}