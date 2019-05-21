using System.Threading;
using System.Threading.Tasks;

namespace Tauron.CQRS.Services
{
    public interface IDispatcherClient
    {
        Task Start(CancellationToken token);
    }
}