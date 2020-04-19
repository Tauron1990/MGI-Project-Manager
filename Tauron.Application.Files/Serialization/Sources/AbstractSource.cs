using System;
using System.IO;
using JetBrains.Annotations;
using Tauron.Application.Files.Serialization.Core;

namespace Tauron.Application.Files.Serialization.Sources
{
    [PublicAPI]
    public abstract class AbstractSource : IStreamSource
    {
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public abstract Stream OpenStream(FileAccess access);

        public abstract IStreamSource OpenSideLocation(string? relativePath);

        protected virtual void Dispose(bool disposing)
        {
        }

        ~AbstractSource() 
            => Dispose(false);
    }
}