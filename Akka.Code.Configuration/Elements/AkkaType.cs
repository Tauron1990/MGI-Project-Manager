using System;
using JetBrains.Annotations;

namespace Akka.Code.Configuration.Elements
{
    [PublicAPI]
    public sealed class AkkaType
    {
        public string Type { get; }

        public AkkaType(string type) => Type = type;

        public static implicit operator AkkaType(string source)
            => new AkkaType(source);

        public static implicit operator AkkaType(Type source)
            => new AkkaType(source.AssemblyQualifiedName);
    }
}