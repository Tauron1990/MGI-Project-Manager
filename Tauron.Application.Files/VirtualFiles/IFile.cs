using System.IO;
using JetBrains.Annotations;

namespace Tauron.Application.Files.VirtualFiles
{
    [PublicAPI]
    public interface IFile : IFileSystemNode
    {
        [NotNull]
        string Extension { get; set; }
        
        long Size { get; }

        [NotNull]
        Stream Open(FileAccess access);

        [NotNull]
        Stream Create();

        [NotNull]
        Stream CreateNew();

        [NotNull]
        IFile MoveTo([NotNull]string location);
    }
}