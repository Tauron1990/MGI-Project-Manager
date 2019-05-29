using CQRSlite.Messages;
using Newtonsoft.Json;
using Tauron.CQRS.Common.Dto.TypeHandling;

namespace Tauron.CQRS.Common.ServerHubs
{
    public class DomainMessage
    {
        public int SequenceNumber { get; set; }

        public string EventName { get; set; }

        [JsonConverter(typeof(TypeResolver))]
        public IMessage EventData { get; set; }

        public EventType EventType { get; set; }

        //public string TypeName { get; set; }
    }
}