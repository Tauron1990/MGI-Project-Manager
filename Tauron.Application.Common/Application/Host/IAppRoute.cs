using System.Threading;
using System.Threading.Tasks;

namespace Tauron.Application.Host
{
    public interface IAppRoute
    {
        Task WaitForStartAsync(CancellationToken cancellationToken);

        Task StopAsync(CancellationToken cancellationToken);
    }
}