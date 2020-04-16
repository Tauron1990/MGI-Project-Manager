using System.Threading.Tasks;
using JetBrains.Annotations;
using Tauron.Application.Files.VirtualFiles;

namespace Tauron.Application.SoftwareRepo
{
    [PublicAPI]
    public interface IRepoFactory
    {
        Task<SoftwareRepository> Create(IDirectory path);
        Task<SoftwareRepository> Read(IDirectory path);
        bool IsValid(IDirectory path);

        Task<SoftwareRepository> Create(string path);

        Task<SoftwareRepository> Read(string path);
        bool IsValid(string path);
    }
}