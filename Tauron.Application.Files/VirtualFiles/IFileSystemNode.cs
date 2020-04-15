using System;
using JetBrains.Annotations;

namespace Tauron.Application.Files.VirtualFiles
{
    [PublicAPI]
    public interface IFileSystemNode
    {
        [NotNull]
        string OriginalPath { get; }

        DateTime LastModified { get; }

        [CanBeNull]
        IDirectory ParentDirectory { get; }

        bool IsDirectory { get; }

        bool Exist { get; }

        void Delete();

        [NotNull]
        string Name { get; }
        
    }
}