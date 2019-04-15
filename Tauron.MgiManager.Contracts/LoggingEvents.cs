using Microsoft.Extensions.Logging;

namespace Tauron.MgiProjectManager
{
    public static class LoggingEvents
    {
        public static readonly EventId InitDatabase = new EventId(101, "Error whilst creating and seeding database");
        public static readonly EventId SendEmail = new EventId(201, "Error whilst sending email");
    }
}