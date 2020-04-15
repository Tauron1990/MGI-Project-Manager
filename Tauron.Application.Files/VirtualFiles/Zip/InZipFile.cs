using System;
using System.IO;
using Ionic.Zip;
using JetBrains.Annotations;
using Tauron.Application.Files.VirtualFiles.Core;

namespace Tauron.Application.Files.VirtualFiles.Zip
{
    public sealed class InZipFile : FileBase<ZipEntry>
    {
        private readonly InternalZipDirectory _directory;
        private readonly ZipEntry? _entry;

        private readonly ZipFile _file;

        public InZipFile(IDirectory parentDirectory, string originalPath, ZipFile file, InternalZipDirectory directory, ZipEntry? entry)
            : base(() => parentDirectory, originalPath, Path.GetFileName(entry?.FileName ?? string.Empty))
        {
            _file = file;
            _directory = directory;
            _entry = entry;
        }


        public override DateTime LastModified => InfoObject.LastModified;

        public override bool Exist => InfoObject == null;

        public override string Extension
        {
            get => InfoObject.FileName.GetExtension();
            set => throw new NotSupportedException();
        }

        public override long Size => InfoObject.UncompressedSize;

        public override IFile MoveTo(string location)
        {
            throw new NotSupportedException();
        }

        protected override void DeleteImpl()
        {
            _file.RemoveEntry(InfoObject);
            Reset(OriginalPath, ParentDirectory);
        }

        protected override ZipEntry? GetInfo(string path)
        {
            return _entry;
        }

        protected override Stream CreateStream(FileAccess access, InternalFileMode mode)
        {
            switch (access)
            {
                case FileAccess.Read:
                    return InfoObject.OpenReader();
                case FileAccess.Write:
                case FileAccess.ReadWrite:
                    using (var stream = new MemoryStream())
                    {
                        if (!Exist) return new ZipWriteHelper(OriginalPath, _file, new byte[0], ZipEntryUpdated);

                        InfoObject.Extract(stream);
                        return new ZipWriteHelper(InfoObject.FileName, _file, stream.GetBuffer(), ZipEntryUpdated);
                    }
                default:
                    throw new ArgumentOutOfRangeException(nameof(access));
            }
        }

        private void ZipEntryUpdated([NotNull] ZipEntry zipEntry)
        {
            _directory.Files.RemoveAt(_directory.Files.FindIndex(ent => ent.FileName == zipEntry.FileName));
            Reset(OriginalPath, ParentDirectory);
        }

        private class ZipWriteHelper : MemoryStream
        {
            private readonly string _entry;
            private readonly ZipFile _file;
            private readonly Action<ZipEntry> _updateAction;

            public ZipWriteHelper(string entry, ZipFile file, byte[] buffer, Action<ZipEntry> updateAction)
                : base(buffer)
            {
                _entry = entry;
                _file = file;
                _updateAction = updateAction;
            }

            protected override void Dispose(bool disposing)
            {
                _updateAction(_file.UpdateEntry(_entry, GetBuffer()));
                base.Dispose(disposing);
            }
        }
    }
}