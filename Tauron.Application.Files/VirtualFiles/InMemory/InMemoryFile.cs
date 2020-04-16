using System;
using System.IO;
using Tauron.Application.Files.VirtualFiles.Core;
using Tauron.Application.Files.VirtualFiles.InMemory.Data;

namespace Tauron.Application.Files.VirtualFiles.InMemory
{
    public sealed class InMemoryFile : FileBase<DataFile>
    {
        private readonly InMemoryDirectory _parentDirectory;
        private readonly DataFile _file;

        public InMemoryFile(InMemoryDirectory parentDirectory, string originalPath, string name) 
            : base(() => parentDirectory, originalPath, name)
        {
            _parentDirectory = parentDirectory;
            _file = parentDirectory.GetOrAddFile(name);
        }

        public override DateTime LastModified => _file.LastModifed;
        public override bool Exist => true;
        protected override void DeleteImpl() 
            => _parentDirectory.Remove(_file);

        protected override DataFile? GetInfo(string path) 
            => _file;

        public override string Extension
        {
            get => Path.GetExtension(_file.Name);
            set => _file.Name = Path.ChangeExtension(_file.Name, value);
        }
        public override IFile MoveTo(string location) 
            => throw new NotSupportedException();

        public override long Size => _file.Data?.Length ?? 0;
        protected override Stream CreateStream(FileAccess access, InternalFileMode mode) 
            => new InMemoryStream(_file);
    }
}