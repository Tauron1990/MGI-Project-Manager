using System.Collections.Generic;
using Akka.Code.Configuration.Elements;
using Akka.Configuration;
using JetBrains.Annotations;

namespace Akka.Code.Configuration
{
    [PublicAPI]
    public sealed class AkkaRootConfiguration : ConfigurationElement
    {
        public AkkaConfiguration Akka => GetAddElement<AkkaConfiguration>("akka");

        public Config CreateConfig()
        {
            var config = Construct();
            return config == null ? ConfigurationFactory.Empty : ConfigurationFactory.FromObject(config);
        }
    }
}