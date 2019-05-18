using System.Threading.Tasks;

namespace Tauron.CQRS.Server.Core
{
    public interface IApiKeyStore
    {
        Task<string> GetServiceFromKey(string apiKey);

        Task<bool> Validate(string apiKey);

        Task<string> Register(string name);
    }
}