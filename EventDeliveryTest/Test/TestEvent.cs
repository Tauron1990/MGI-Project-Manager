using System;
using CQRSlite.Events;

namespace EventDeliveryTest.Test
{
    public class TestEvent : IEvent
    {
        public string Result { get; set; }

        public Guid Id { get; set; }

        public int Version { get; set; }

        public DateTimeOffset TimeStamp { get; set; }
    }
}