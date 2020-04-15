using System;
using System.Runtime.Serialization;
using JetBrains.Annotations;

namespace Tauron.Application.Files.Serialization
{
    [Serializable]
    [PublicAPI]
    public class SerializerElementNullException : Exception
    {
        public SerializerElementNullException()
        {
        }

        public SerializerElementNullException(string message) : base(message)
        {
        }

        public SerializerElementNullException(string message, Exception inner) : base(message, inner)
        {
        }

        protected SerializerElementNullException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}