using Tauron.CQRS.Common.ServerHubs;

namespace Tauron.CQRS.Server.Hubs
{
    public class RecivedDomainEvent
    {
        public DomainEvent DomainEvent { get; }

        public string ApiKey { get; }

        public RecivedDomainEvent(DomainEvent domainEvent, string apiKey)
        {
            DomainEvent = domainEvent;
            ApiKey = apiKey;
        }
    }
}