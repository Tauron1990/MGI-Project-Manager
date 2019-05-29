using Tauron.CQRS.Common.ServerHubs;

namespace Tauron.CQRS.Server.Hubs
{
    public class RecivedDomainEvent
    {
        public DomainMessage RealMessage { get; }

        public string ApiKey { get; }

        public RecivedDomainEvent(DomainMessage realMessage, string apiKey)
        {
            RealMessage = realMessage;
            ApiKey = apiKey;
        }
    }
}