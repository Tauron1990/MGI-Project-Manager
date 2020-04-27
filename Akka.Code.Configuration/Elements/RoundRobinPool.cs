using JetBrains.Annotations;

namespace Akka.Code.Configuration.Elements
{
    [PublicAPI]
    public sealed class RoundRobinPool : PoolConfigBase
    {
        public RoundRobinPool() 
            : base("round-robin-pool")
        {
        }
    }
}