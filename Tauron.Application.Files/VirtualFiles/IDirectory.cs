using System.Collections.Generic;
using JetBrains.Annotations;

namespace Tauron.Application.Files.VirtualFiles
{
    [PublicAPI]
    public interface IDirectory : IFileSystemNode
    {
        IEnumerable<IDirectory> Directories { get; }

        IEnumerable<IFile> Files { get; }

        IFile GetFile(string name);

        IDirectory GetDirectory(string name);

        IDirectory MoveTo(string location);
    }
}