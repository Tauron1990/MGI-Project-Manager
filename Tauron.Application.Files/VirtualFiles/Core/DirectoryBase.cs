using System;
using System.Collections.Generic;

namespace Tauron.Application.Files.VirtualFiles.Core
{
    public abstract class DirectoryBase<TInfo> : FileSystemNodeBase<TInfo>, IDirectory where TInfo : class
    {
        protected DirectoryBase(Func<IDirectory?> parentDirectory, string originalPath, string name)
            : base(parentDirectory, originalPath, name)
        {
        }

        public abstract IDirectory GetDirectory(string name);
        public abstract IEnumerable<IDirectory> Directories { get; }
        public abstract IEnumerable<IFile> Files { get; }
        public abstract IFile GetFile(string name);
        public abstract IDirectory MoveTo(string location);
    }
}