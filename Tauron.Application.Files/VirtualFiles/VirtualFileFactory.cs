using Ionic.Zip;
using JetBrains.Annotations;
using Tauron.Application.Files.VirtualFiles.Zip;

namespace Tauron.Application.Files.VirtualFiles
{
    [PublicAPI]
    public static class VirtualFileFactory
    {
        public static IVirtualFileSystem CrerateLocal(string path) => new LocalFileSystem.LocalFileSystem(Argument.NotNull(path, nameof(path)));

        public static IVirtualFileSystem CreateZip(string path) => new InZipFileSystem(ZipFile.Read(Argument.NotNull(path, nameof(path))));
    }
}