using System.Threading.Tasks;
using Grpc.Core;

namespace Tauron.Application.ToolUi.SimpleAuth
{
    public interface IClientFactory<out TClient>
        where TClient : ClientBase
    {
        Task<TClient> Create(string host);
    }
}