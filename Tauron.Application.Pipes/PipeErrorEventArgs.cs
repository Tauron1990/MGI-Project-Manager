using System;

namespace Tauron.Application.Pipes
{
    public sealed class PipeErrorEventArgs : EventArgs
    {
        public PipeErrorEventArgs(Exception error, bool @continue)
        {
            Error = error;
            Continue = @continue;
        }

        public Exception Error { get; }

        public bool Continue { get; set; }
    }
}