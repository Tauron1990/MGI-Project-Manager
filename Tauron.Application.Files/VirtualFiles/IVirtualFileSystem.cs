using System;
using JetBrains.Annotations;

namespace Tauron.Application.Files.VirtualFiles
{
    public interface IVirtualFileSystem : IDirectory, IDisposable
    {
        bool IsRealTime       { get; }
        bool SaveAfterDispose { get; set; }

        [NotNull]
        string Source { get; }

        void Reload([NotNull] string source);
    }
}