using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Tauron.Application.Files.VirtualFiles.Core;

namespace Tauron.Application.Files.VirtualFiles.LocalFileSystem
{
    public class LocalDirectory : DirectoryBase<DirectoryInfo>
    {
        public LocalDirectory(string fullPath, Func<IDirectory?> parentDirectory)
            : base(parentDirectory, fullPath, fullPath.GetFileName())
        {
        }

        public LocalDirectory(string fullPath)
            : base(() => GetParentDirectory(fullPath), fullPath, fullPath.GetFileName())
        {
        }

        public override DateTime LastModified => InfoObject?.LastWriteTime ?? DateTime.MinValue;

        public override bool Exist => InfoObject?.Exists ?? false;

        public override IDirectory GetDirectory(string name)
        {
            var dic = new LocalDirectory(Path.Combine(OriginalPath, name), () => this);
            if(!dic.Exist)
                dic.InfoObject?.Create();

            return dic;
        }

        public override IEnumerable<IDirectory> Directories => Directory.EnumerateDirectories(OriginalPath).Select(str => new LocalDirectory(str, () => this));

        public override IEnumerable<IFile> Files => Directory.EnumerateFiles(OriginalPath).Select(str => new LocalFile(str, this));

        private static IDirectory? GetParentDirectory(string fullpath)
        {
            string name = Path.GetDirectoryName(fullpath);
            return string.IsNullOrEmpty(name) ? null : new LocalDirectory(fullpath);
        }

        protected override void DeleteImpl() 
            => InfoObject?.Delete(true);

        protected override DirectoryInfo GetInfo(string path) 
            => new DirectoryInfo(path);

        public override IFile GetFile(string name) 
            => new LocalFile(OriginalPath.CombinePath(name), this);

        public override IDirectory MoveTo(string location)
        {
            if (location == OriginalPath) return this;

            OriginalPath.MoveTo(location);

            return new LocalDirectory(location);
        }
    }
}