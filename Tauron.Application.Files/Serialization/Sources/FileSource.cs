using System.IO;
using JetBrains.Annotations;
using Tauron.Application.Files.Serialization.Core;

namespace Tauron.Application.Files.Serialization.Sources
{
    [PublicAPI]
    public sealed class FileSource : AbstractSource
    {
        private readonly string _file;

        public FileSource(string file)
        {
            _file = Argument.NotNull(file, nameof(file));
        }

        public override Stream OpenStream(FileAccess access)
        {
            return new FileStream(_file, access.HasFlag(FileAccess.Read) ? FileMode.Open : FileMode.Create, access, FileShare.None);
        }

        public override IStreamSource OpenSideLocation(string? relativePath)
        {
            return new FileSource(_file.GetDirectoryName().CombinePath(Argument.NotNull(relativePath, nameof(relativePath))));
        }
    }
}