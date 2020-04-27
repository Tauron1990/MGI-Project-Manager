using JetBrains.Annotations;

namespace Akka.Code.Configuration.Elements
{
    [PublicAPI]
    public sealed class BroadcastPoolConfiguration : PoolConfigBase
    {
        public BroadcastPoolConfiguration() 
            : base("broadcast-pool")
        {
        }
    }
}