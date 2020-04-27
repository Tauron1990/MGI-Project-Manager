using JetBrains.Annotations;

namespace Akka.Code.Configuration.Elements
{
    [PublicAPI]
    public abstract class PoolConfigBase : RouterConfigBase
    {

        protected PoolConfigBase(AkkaType type)
            : base(type) { }

        public ResizerConfiguration Resizer => GetAddElement<ResizerConfiguration>("resizer");

        public int NrOfInstances
        {
            get => Get<int>("nr-of-instances");
            set => Set(value, "nr-of-instances");
        }
    }
}