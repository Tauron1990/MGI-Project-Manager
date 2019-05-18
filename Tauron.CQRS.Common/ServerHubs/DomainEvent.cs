namespace Tauron.CQRS.Common.ServerHubs
{
    public class DomainEvent
    {
        public int SequenceNumber { get; set; }

        public string EventName { get; set; }

        public string EventData { get; set; }

        public EventType EventType { get; set; }
    }
}