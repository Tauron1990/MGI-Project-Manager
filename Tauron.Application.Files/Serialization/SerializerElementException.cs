using System;
using System.Runtime.Serialization;
using JetBrains.Annotations;

namespace Tauron.Application.Files.Serialization
{
    [Serializable]
    [PublicAPI]
    public class SerializerElementException : Exception
    {
        public SerializerElementException() { }

        public SerializerElementException([NotNull] string message) : base(message) { }

        public SerializerElementException([NotNull] string message, [NotNull] Exception inner) : base(message, inner) { }

        protected SerializerElementException([NotNull] SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}