using System.Threading;
using System.Threading.Tasks;

namespace Tauron.CQRS.Services.Core
{
    public interface IHandlerManager
    {
        Task Init(CancellationToken token);
    }
}