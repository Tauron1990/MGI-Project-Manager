using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace JKang.IpcServiceFramework
{
    [PublicAPI]
    public interface IIpcServiceHost
    {
        void Run();

        Task RunAsync(CancellationToken cancellationToken = default);
    }
}