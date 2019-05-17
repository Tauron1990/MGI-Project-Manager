namespace Tauron.CQRS.Common.ServerHubs
{
    public class DomainEvent
    {
        public string EventName { get; set; }

        public string EventData { get; set; }
    }
}