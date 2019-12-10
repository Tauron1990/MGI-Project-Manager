using System;
using System.Threading.Tasks;

namespace Tauron.Application.Pipes.IO
{
    public interface IPipe : IDisposable, IAsyncDisposable
    {
        event Func<(Exception Exception, bool OnReader), Task<bool>>? OnError; 

        bool CanRead { get; }

        bool CanWrite { get; }

        Task Init(Func<byte[], int, Task> readHandler);

        Task Write( data);
    }
}