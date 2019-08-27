using System;
using CQRSlite.Events;
using Tauron.CQRS.Common.ServerHubs;

namespace EventDeliveryTest.Test
{
    public class TestEvent : CQRSEvent
    {
        public string Result { get; set; }

        public TestEvent(Guid id, int version, string result) : base(id, version)
        {
        }
    }
}