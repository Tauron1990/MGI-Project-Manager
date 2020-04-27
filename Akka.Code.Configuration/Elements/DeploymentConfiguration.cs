using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace Akka.Code.Configuration.Elements
{
    [PublicAPI]
    public sealed class DeploymentConfiguration : ConfigurationElement
    {
        public IEnumerable<SingleActorConfiguration> Actors => ToAddElements.Select(p => p.Value).OfType<SingleActorConfiguration>();

        public SingleActorConfiguration AddActor(string path) => GetAddElement<SingleActorConfiguration>(path);
    }
}