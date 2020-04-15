using System;
using System.IO;
using JetBrains.Annotations;

namespace Tauron.Application.Files.Serialization.Core
{
    public interface IStreamSource : IDisposable
    {
        [NotNull]
        Stream OpenStream(FileAccess access);

        [CanBeNull]
        IStreamSource OpenSideLocation([CanBeNull] string relativePath);
    }
}