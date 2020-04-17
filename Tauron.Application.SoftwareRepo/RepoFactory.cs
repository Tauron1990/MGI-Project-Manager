using System.Threading.Tasks;
using Tauron.Application.Files.VirtualFiles;

namespace Tauron.Application.SoftwareRepo
{
    public sealed class RepoFactory : IRepoFactory
    {
        public async Task<SoftwareRepository> Create(IDirectory path)
        {
            var temp = new SoftwareRepository(path);
            await temp.InitNew();
            return temp;
        }

        public async Task<SoftwareRepository> Read(IDirectory path)
        {
            var temp = new SoftwareRepository(path);
            await temp.Init();
            return temp;
        }

        public bool IsValid(IDirectory path) 
            => path.GetFile(SoftwareRepository.FileName).Exist;

        public Task<SoftwareRepository> Create(string path) 
            => Create(VirtualFileFactory.CrerateLocal(path));

        public Task<SoftwareRepository> Read(string path) 
            => Read(VirtualFileFactory.CrerateLocal(path));

        public bool IsValid(string path)
            => IsValid(VirtualFileFactory.CrerateLocal(path));
    }  
}