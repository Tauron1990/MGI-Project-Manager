using Tauron.CQRS.Common.ServerHubs;

namespace Tauron.CQRS.Server.Hubs
{
    public class RecivedDomainEvent
    {
        public ServerDomainMessage RealMessage { get; }

        public string ApiKey { get; }

        public RecivedDomainEvent(ServerDomainMessage realMessage, string apiKey)
        {
            RealMessage = realMessage;
            ApiKey = apiKey;
        }
    }
}