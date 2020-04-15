using Ionic.Zip;
using JetBrains.Annotations;
using Tauron.Application.Files.VirtualFiles.Zip;

namespace Tauron.Application.Files.VirtualFiles
{
    [PublicAPI]
    public static class VirtualFileFactory
    {
        public static IVirtualFileSystem CrerateLocal(string path)
        {
            return new LocalFileSystem.LocalFileSystem(Argument.NotNull(path, nameof(path)));
        }

        public static IVirtualFileSystem CreateZip(string path)
        {
            return new InZipFileSystem(ZipFile.Read(Argument.NotNull(path, nameof(path))));
        }
    }
}