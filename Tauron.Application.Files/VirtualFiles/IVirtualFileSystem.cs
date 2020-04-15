using System;
using JetBrains.Annotations;

namespace Tauron.Application.Files.VirtualFiles
{
    [PublicAPI]
    public interface IVirtualFileSystem : IDirectory, IDisposable
    {
        bool IsRealTime { get; }
        bool SaveAfterDispose { get; set; }

        string Source { get; }

        void Reload([NotNull] string source);
    }
}