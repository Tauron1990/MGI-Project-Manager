using JetBrains.Annotations;

namespace Akka.Code.Configuration.Elements
{
    [PublicAPI]
    public sealed class DebugConfiguration : ConfigurationElement
    {
        public bool Receive
        {
            get => Get<bool>("receive");
            set => Set(value, "receive");
        }
        public bool Autoreceive
        {
            get => Get<bool>("autoreceive");
            set => Set(value, "autoreceive");
        }
        public bool Lifecycle
        {
            get => Get<bool>("lifecycle");
            set => Set(value, "lifecycle");
        }
        public bool EventStream
        {
            get => Get<bool>("event-stream");
            set => Set(value, "event-stream");
        }
        public bool Unhandled
        {
            get => Get<bool>("unhandled");
            set => Set(value, "unhandled");
        }
    }
}