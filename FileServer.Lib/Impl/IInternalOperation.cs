using System;

namespace FileServer.Lib.Impl
{
    public interface IInternalOperation : IDisposable
    {
        bool IsCollected { get; }

        TimeSpan Timeout { get; }
    }
}