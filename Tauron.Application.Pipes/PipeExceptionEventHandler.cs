using System;
using System.Threading.Tasks;

namespace Tauron.Application.Pipes
{
    /// <summary>
    ///     Handles exceptions thrown during a read or write operation on a named pipe.
    /// </summary>
    /// <param name="exception">Exception that was thrown</param>
    public delegate Task PipeExceptionEventHandler(Exception exception);
}