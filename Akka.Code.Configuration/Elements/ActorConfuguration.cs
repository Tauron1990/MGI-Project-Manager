﻿using JetBrains.Annotations;

namespace Akka.Code.Configuration.Elements
{
    [PublicAPI]
    public sealed class ActorConfuguration : ConfigurationElement
    {
        public DebugConfiguration Debug => GetAddElement<DebugConfiguration>("debug");

        public AkkaType? Provider
        {
            get => Get<AkkaType>("provider");
            set => Set(value, "provider");
        }
    }
}