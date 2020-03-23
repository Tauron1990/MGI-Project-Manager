using System.Threading.Tasks;

namespace Tauron.Application.SimpleAuth.Core
{
    public interface IPasswordVault
    {
        Task<bool> CheckPassword(string pass);
        Task<bool> SetPassword(string pass);
    }
}