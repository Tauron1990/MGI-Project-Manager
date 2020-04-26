using System;

namespace Akka.Copy.Messages
{
    public sealed class RecuverErrorMessage
    {
        public Exception Error { get; }

        public RecuverErrorMessage(Exception error) => Error = error;
    }
}