using System;
using System.IO;
using Tauron.Application.Files.VirtualFiles.Core;
using Tauron.Application.Files.VirtualFiles.InMemory.Data;

namespace Tauron.Application.Files.VirtualFiles.InMemory
{
    public sealed class InMemoryFile : FileBase<DataFile>
    {
        private readonly InMemoryDirectory _parentDirectory;

        public InMemoryFile(InMemoryDirectory parentDirectory, string originalPath, string name) 
            : base(() => parentDirectory, originalPath, name)
        {
            _parentDirectory = parentDirectory;
        }

        public override DateTime LastModified => InfoObject?.LastModifed ?? DateTime.MinValue;
        public override bool Exist => _parentDirectory.ExistFile(Name);
        protected override void DeleteImpl() 
            => _parentDirectory.Remove(InfoObject);

        protected override DataFile? GetInfo(string path) 
            => _parentDirectory.GetOrAddFile(Name);

        public override string Extension
        {
            get => Path.GetExtension(InfoObject?.Name);
            set
            {
                var data = InfoObject;
                if(data == null) return;
                data.Name = Path.ChangeExtension(data.Name, value);

                Name = data.Name;
            }
        }

        public override IFile MoveTo(string location) 
            => throw new NotSupportedException();

        public override long Size => InfoObject?.Data?.Length ?? 0;
        protected override Stream CreateStream(FileAccess access, InternalFileMode mode)
        {
            var data = InfoObject;
            if(data == null) 
                throw new InvalidOperationException("");

            return new InMemoryStream(data);
        }
    }
}