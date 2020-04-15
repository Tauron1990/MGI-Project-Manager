using System;
using System.Runtime.Serialization;
using JetBrains.Annotations;

namespace Tauron.Application.Files.Serialization
{
    [Serializable]
    [PublicAPI]
    public class SerializerElementException : Exception
    {
        public SerializerElementException()
        {
        }

        public SerializerElementException(string message) : base(message)
        {
        }

        public SerializerElementException(string message, Exception inner) : base(message, inner)
        {
        }

        protected SerializerElementException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}