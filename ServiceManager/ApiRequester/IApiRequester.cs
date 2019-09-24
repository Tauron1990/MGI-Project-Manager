using System.Threading.Tasks;
using RestEase;

namespace ServiceManager.ApiRequester
{
    public interface IApiRequester
    {
        [Get(nameof(RegisterApiKey))]
        Task<string> RegisterApiKey(string serviceName);
    }
}