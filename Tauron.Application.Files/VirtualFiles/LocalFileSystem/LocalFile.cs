using System;
using System.IO;
using JetBrains.Annotations;
using Tauron.Application.Files.VirtualFiles.Core;

namespace Tauron.Application.Files.VirtualFiles.LocalFileSystem
{
    public class LocalFile : FileBase<FileInfo>
    {
        public LocalFile(string fullPath, IDirectory path)
            : base(() => path, fullPath, fullPath.GetFileName())
        {
        }

        private LocalFile(string fullPath)
            : base(() => new LocalDirectory(fullPath.GetDirectoryName()), fullPath, fullPath.GetFileName())
        {
        }

        public override DateTime LastModified => InfoObject.LastWriteTime;

        public override bool Exist => InfoObject.Exists;

        public override string Extension
        {
            get => InfoObject.Extension;
            set
            {
                if (InfoObject.Extension == value) return;

                MoveFile(InfoObject, Path.ChangeExtension(OriginalPath, value));
            }
        }

        public override long Size => InfoObject.Length;

        public override IFile MoveTo(string location)
        {
            if (InfoObject.FullName == location) return this;

            MoveFile(InfoObject, location);

            return new LocalFile(location);
        }

        protected override void DeleteImpl()
        {
            InfoObject.Delete();
        }

        protected override FileInfo GetInfo(string path)
        {
            return new FileInfo(path);
        }

        protected override Stream CreateStream(FileAccess access, InternalFileMode mode)
        {
            return new FileStream(OriginalPath, (FileMode) mode, access, access == FileAccess.Read ? FileShare.Read : FileShare.None);
        }

        private void MoveFile([NotNull] FileInfo old, [NotNull] string newLoc)
        {
            old.MoveTo(newLoc);
        }
    }
}