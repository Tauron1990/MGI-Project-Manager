using System;
using System.IO;

namespace Tauron.Application.Files.VirtualFiles.Core
{
    public abstract class FileBase<TInfo> : FileSystemNodeBase<TInfo>, IFile where TInfo : class
    {
        protected FileBase(Func<IDirectory> parentDirectory, string originalPath, string name)
            : base(parentDirectory, originalPath, name)
        {
        }

        public abstract string Extension { get; set; }

        public virtual Stream Open(FileAccess access) 
            => CreateStream(access, InternalFileMode.Open);

        public virtual Stream Create() 
            => CreateStream(FileAccess.Write, InternalFileMode.Create);

        public virtual Stream CreateNew() 
            => CreateStream(FileAccess.Write, InternalFileMode.CreateNew);

        public abstract IFile MoveTo(string location);

        public abstract long Size { get; }

        protected abstract Stream CreateStream(FileAccess access, InternalFileMode mode);

        protected enum InternalFileMode
        {
            Open = 3,
            Create = 2,
            CreateNew = 1
        }
    }
}