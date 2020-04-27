using JetBrains.Annotations;

namespace Akka.Code.Configuration.Elements
{
    [PublicAPI]
    public sealed class SingleActorConfiguration : ConfigurationElement
    {
        public AkkaType? Dispatcher
        {
            get => Get<AkkaType>("dispatcher");
            set => Set(value, "dispatcher");
        }

        public RouterConfigBase? Router
        {
            get => TryGetMergeElement<RouterConfigBase>();
            set => ReplaceMerge(value);
        }
    }
}