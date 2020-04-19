using System;
using System.IO;
using Tauron.Application.Files.Serialization.Core;

namespace Tauron.Application.Files.Serialization.Sources
{
    public sealed class EmptySource : IStreamSource
    {
        public static readonly EmptySource Instance = new EmptySource();

        public void Dispose()
        {
        }

        public Stream OpenStream(FileAccess access) => Stream.Null;

        public IStreamSource? OpenSideLocation(string? relativePath) => throw new NotSupportedException();
    }
}