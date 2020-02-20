using System;

namespace FileServer.Lib.Impl
{
    public interface IInternalOperation : IDisposable
    {
        TimeSpan Timeout { get; }
    }
}