using Tauron.CQRS.Common.ServerHubs;

namespace Tauron.CQRS.Server.Hubs
{
    public class RecivedDomainEvent
    {
        public DomainEvent RealEvent { get; }

        public string ApiKey { get; }

        public RecivedDomainEvent(DomainEvent realEvent, string apiKey)
        {
            RealEvent = realEvent;
            ApiKey = apiKey;
        }
    }
}