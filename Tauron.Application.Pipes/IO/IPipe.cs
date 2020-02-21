using System;
using System.Threading.Tasks;

namespace Tauron.Application.Pipes.IO
{
    public interface IPipe : IDisposable
    {
        bool CanRead { get; }

        bool CanWrite { get; }
        event Func<(Exception Exception, bool OnReader), Task<bool>>? OnError;

        Task Init(Func<byte[], int, Task> readHandler);

        Task Write(byte[] segment);
    }
}