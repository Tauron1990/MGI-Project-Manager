using JetBrains.Annotations;

namespace Akka.Code.Configuration.Elements
{
    [PublicAPI]
    public sealed class ConsistentHashingPool : PoolConfigBase
    {
        public ConsistentHashingPool() : base("consistent-hashing-pool")
        {
        }

        public int VirtualNodesFactor
        {
            get => Get<int>("virtual-nodes-factor");
            set => Set(value, "virtual-nodes-factor");
        }
    }
}