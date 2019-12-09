using System;

namespace Tauron.Application.Pipes
{
    public sealed class PipeErrorEventArgs : EventArgs
    {
        public Exception Error { get; }

        public bool Continue { get; set; }

        public PipeErrorEventArgs(Exception error, bool @continue)
        {
            Error = error;
            Continue = @continue;
        }
    }
}