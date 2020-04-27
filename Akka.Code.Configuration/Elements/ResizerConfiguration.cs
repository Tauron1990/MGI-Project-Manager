using JetBrains.Annotations;

namespace Akka.Code.Configuration.Elements
{
    [PublicAPI]
    public sealed class ResizerConfiguration : ConfigurationElement
    {
        public bool Enabled
        {
            get => Get<bool>("enabled");
            set => Set(value, "enabled");
        }

        public int MessagesPerResize
        {
            get => Get<int>("messages-per-resize");
            set => Set(value, "messages-per-resize");
        }
        public double BackoffRate
        {
            get => Get<double>("backoff-rate");
            set => Set(value, "backoff-rate");
        }
        public double BackoffThreshold
        {
            get => Get<double>("backoff-threshold");
            set => Set(value, "backoff-threshold");
        }
        public double RampupRate
        {
            get => Get<double>("rampup-rate");
            set => Set(value, "rampup-rate");
        }
        public int PressureThreshold
        {
            get => Get<int>("pressure-threshold");
            set => Set(value, "pressure-threshold");
        }
        public int UpperBound
        {
            get => Get<int>("upper-bound");
            set => Set(value, "upper-bound");
        }
        public int LowerBound
        {
            get => Get<int>("lower-bound");
            set => Set(value, "lower-bound");
        }
    }
}