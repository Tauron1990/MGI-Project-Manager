using System;
using JetBrains.Annotations;
using Tauron.Application.Files.VirtualFiles.InMemory.Data;

namespace Tauron.Application.Files.VirtualFiles.InMemory
{
    public sealed class InMemoryFileSystem : InMemoryDirectory, IVirtualFileSystem
    {
        public InMemoryFileSystem(string originalPath, string name, DataDirectory dic) 
            : base(null, originalPath, name, dic)
        {
        }

        public void Dispose()
        {
        }

        public bool IsRealTime => true;
        public bool SaveAfterDispose
        {
            get => false;
            set => throw new NotSupportedException();
        }
        public string Source => Name;
        public void Reload(string source) 
            => throw new NotSupportedException();
    }
}