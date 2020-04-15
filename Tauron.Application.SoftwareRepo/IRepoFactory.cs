using System.Threading.Tasks;

namespace Tauron.Application.SoftwareRepo
{
    public interface IRepoFactory
    {
        Task<SoftwareRepository> Create(string path);
        Task<SoftwareRepository> Read(string path);
        bool IsValid(string path);
    }
}