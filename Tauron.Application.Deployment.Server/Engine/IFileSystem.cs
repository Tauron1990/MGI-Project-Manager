using Tauron.Application.Files.VirtualFiles;

namespace Tauron.Application.Deployment.Server.Engine
{
    public interface IFileSystem
    {
        IDirectory RepositoryRoot { get; }
    }
}