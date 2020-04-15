using System.Collections.Generic;
using JetBrains.Annotations;

namespace Tauron.Application.Files.VirtualFiles
{
    [PublicAPI]
    public interface IDirectory : IFileSystemNode
    {
        IEnumerable<IDirectory> Directories { get; }

        IEnumerable<IFile> Files { get; }

        IFile GetFile([NotNull] string name);

        IDirectory MoveTo([NotNull] string location);
    }
}