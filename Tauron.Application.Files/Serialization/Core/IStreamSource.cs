using System;
using System.IO;

namespace Tauron.Application.Files.Serialization.Core
{
    public interface IStreamSource : IDisposable
    {
        Stream OpenStream(FileAccess access);

        IStreamSource? OpenSideLocation(string? relativePath);
    }
}