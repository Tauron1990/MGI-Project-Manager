using System.IO;
using JetBrains.Annotations;

namespace Tauron.Application.Files.VirtualFiles
{
    [PublicAPI]
    public interface IFile : IFileSystemNode
    {
        string Extension { get; set; }

        long Size { get; }

        Stream Open(FileAccess access);

        Stream Create();

        Stream CreateNew();

        IFile MoveTo([NotNull] string location);
    }
}