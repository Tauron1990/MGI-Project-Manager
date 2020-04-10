using System.IO;
using System.Threading.Tasks;

namespace Tauron.Application.SoftwareRepo
{
    public sealed class RepoFactory : IRepoFactory
    {
        public async Task<SoftwareRepository> Create(string path)
        {
            var temp = new SoftwareRepository(path);
            await temp.InitNew();
            return temp;
        }

        public async Task<SoftwareRepository> Read(string path)
        {
            var temp = new SoftwareRepository(path);
            await temp.Init();
            return temp;
        }

        public bool IsValid(string path) => File.Exists(Path.Combine(path, SoftwareRepository.FileName));
    }
}