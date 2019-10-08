using System.Threading.Tasks;

namespace Tauron.CQRS.Server.Core
{
    public interface IApiKeyStore
    {
        //Task<string> GetServiceFromKey(string apiKey);

        Task<(bool Ok, string ServiceName)> Validate(string apiKey);

        Task<string> Register(string name);
    }
}