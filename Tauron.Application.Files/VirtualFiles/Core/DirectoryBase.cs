using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Tauron.Application.Files.VirtualFiles.Core
{
    public abstract class DirectoryBase<TInfo> : FileSystemNodeBase<TInfo>, IDirectory
    {
        protected DirectoryBase([CanBeNull] Func<IDirectory> parentDirectory, [NotNull] string originalPath, [NotNull] string name)
            : base(parentDirectory, true, originalPath, name) { }

        public abstract IEnumerable<IDirectory> Directories { get; }
        public abstract IEnumerable<IFile> Files { get; }
        public abstract IFile GetFile(string name);
        public abstract IDirectory MoveTo(string location);
    }
}