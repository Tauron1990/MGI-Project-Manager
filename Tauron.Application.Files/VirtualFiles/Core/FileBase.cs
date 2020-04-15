using System;
using System.IO;
using JetBrains.Annotations;

namespace Tauron.Application.Files.VirtualFiles.Core
{
    public abstract class FileBase<TInfo> : FileSystemNodeBase<TInfo>, IFile
    {
        protected enum InternalFileMode
        {
            Open = 3,
            Create = 2,
            CreateNew = 1
        }

        protected FileBase([NotNull] Func<IDirectory> parentDirectory, [NotNull] string originalPath, [NotNull] string name)
            : base(parentDirectory, true, originalPath, name) { }

        public abstract string Extension { get; set; }
        
        public virtual Stream Open(FileAccess access) => CreateStream(access, InternalFileMode.Open);

        public virtual Stream Create() => CreateStream(FileAccess.Write, InternalFileMode.Create);

        public virtual Stream CreateNew() => CreateStream(FileAccess.Write, InternalFileMode.CreateNew);

        public abstract IFile MoveTo(string location);

        public abstract long Size { get; }

        [NotNull]
        protected abstract Stream CreateStream(FileAccess access, InternalFileMode mode);
    }
}