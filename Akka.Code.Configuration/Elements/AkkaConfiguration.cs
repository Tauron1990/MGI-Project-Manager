using System;
using JetBrains.Annotations;

namespace Akka.Code.Configuration.Elements
{
    [PublicAPI]
    public sealed class AkkaConfiguration : ConfigurationElement
    {
        public ArrayElement<string> Loggers => GetOrAdd("loggers", () => new ArrayElement<string>());

        public AkkaLogLevel StdoutLoglevel
        {
            get => Get<AkkaLogLevel>("stdout-loglevel");
            set => Set(value, "stdout-loglevel");
        }

        public AkkaLogLevel LogLevel
        {
            get => Get<AkkaLogLevel>("loglevel");
            set => Set(value, "loglevel");
        }

        public bool LogConfigOnStart
        {
            get => Get<bool>("log-config-on-start");
            set => Set(value, "log-config-on-start");
        }

        public ActorConfuguration Actor => GetAddElement<ActorConfuguration>("actor");
    }
}