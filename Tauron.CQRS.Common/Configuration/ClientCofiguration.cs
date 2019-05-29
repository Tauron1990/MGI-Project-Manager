namespace Tauron.CQRS.Common.Configuration
{
    public class ClientCofiguration : CommonConfiguration
    {
        public string EventHubUrl { get; set; }

        public string EventServerApiUrl { get; set; }

        public string PersistenceApiUrl { get; set; }

        public string ApiKey { get; set; }
    }
}