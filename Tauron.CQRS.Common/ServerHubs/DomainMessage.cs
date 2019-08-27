using System;

namespace Tauron.CQRS.Common.ServerHubs
{
    public class ServerDomainMessage
    {
        public long SequenceNumber { get; set; }

        public string EventName { get; set; }

        public string EventData { get; set; }

        public EventType EventType { get; set; }
        
        public int Version { get; set; }

        public DateTimeOffset TimeStamp { get; set; }

        public Guid Id { get; set; }

        public string TypeName { get; set; }
    }

    //public class DomainMessage
    //{
    //    public long SequenceNumber { get; set; }

    //    public string EventName { get; set; }

    //    [JsonConverter(typeof(TypeResolver))]
    //    public IMessage EventData { get; set; }

    //    public EventType EventType { get; set; }

    //    public int Version { get; set; }

    //    public DateTimeOffset TimeStamp { get; set; }

    //    public Guid Id { get; set; }

    //    //public string TypeName { get; set; }
    //}
}