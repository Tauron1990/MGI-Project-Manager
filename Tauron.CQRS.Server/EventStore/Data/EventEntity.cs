﻿using System.ComponentModel.DataAnnotations;
using Tauron.CQRS.Common.ServerHubs;

namespace Tauron.CQRS.Server.EventStore.Data
{
    public class EventEntity
    {
        [Key]
        public int SequenceNumber { get; set; }

        public EventType EventType { get; set; }

        public string Data { get; set; }

        public string EventName { get; set; }

        public string Origin { get; set; }

        public EventStatus EventStatus { get; set; }
    }
}