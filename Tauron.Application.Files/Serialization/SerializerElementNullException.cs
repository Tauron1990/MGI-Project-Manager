using System;
using System.Runtime.Serialization;
using JetBrains.Annotations;

namespace Tauron.Application.Files.Serialization
{
    [Serializable]
    [PublicAPI]
    public class SerializerElementNullException : Exception
    {
        public SerializerElementNullException() { }

        public SerializerElementNullException([NotNull] string message) : base(message) { }

        public SerializerElementNullException([NotNull] string message, [NotNull] Exception inner) : base(message, inner) { }

        protected SerializerElementNullException([NotNull] SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}