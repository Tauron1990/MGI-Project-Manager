using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using Tauron.Application.Files.VirtualFiles.Core;

namespace Tauron.Application.Files.VirtualFiles.LocalFileSystem
{
    public class LocalDirectory : DirectoryBase<DirectoryInfo>
    {
        public LocalDirectory([NotNull] string fullPath, [CanBeNull] Func<IDirectory> parentDirectory)
            : base(parentDirectory, fullPath,  fullPath.GetFileName()) { }

        public LocalDirectory(string fullPath) 
            : base(() => GetParentDirectory(fullPath), fullPath, fullPath.GetFileName()) { }

        private static IDirectory GetParentDirectory(string fullpath)
        {
            string name = Path.GetDirectoryName(fullpath);
            return string.IsNullOrEmpty(name) ? null : new LocalDirectory(fullpath);
        }

        public override DateTime LastModified => InfoObject.LastWriteTime;

        public override bool Exist => InfoObject.Exists;

        public override IEnumerable<IDirectory> Directories => Directory.EnumerateDirectories(OriginalPath).Select(str => new LocalDirectory(str, () => this));

        public override IEnumerable<IFile> Files => Directory.EnumerateFiles(OriginalPath).Select(str => new LocalFile(str, this));

        protected override void DeleteImpl() => InfoObject.Delete(true);

        protected override DirectoryInfo GetInfo(string path) => new DirectoryInfo(path);

        public override IFile GetFile(string name) => new LocalFile(OriginalPath.CombinePath(name), this);

        public override IDirectory MoveTo(string location)
        {
            if (location == OriginalPath) return this;

            OriginalPath.MoveTo(location);

            return new LocalDirectory(location);
        }
    }
}