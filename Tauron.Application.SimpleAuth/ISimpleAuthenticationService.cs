using System.Threading.Tasks;

namespace Tauron.Application.SimpleAuth
{
    public interface ISimpleAuthenticationService
    {
        Task<bool> IsValidUserAsync(string user, string password);
    }
}