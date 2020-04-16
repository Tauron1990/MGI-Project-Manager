using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using Tauron.Application.Files.VirtualFiles.Core;
using Tauron.Application.Files.VirtualFiles.InMemory.Data;

namespace Tauron.Application.Files.VirtualFiles.InMemory
{
    public sealed class InMemoryDirectory : DirectoryBase<DataDirectory>
    {
        private readonly InMemoryDirectory? _parentDirectory;
        private readonly DataDirectory _dic;

        public InMemoryDirectory(InMemoryDirectory? parentDirectory, string originalPath, string name, DataDirectory dic) : base(() => parentDirectory, originalPath, name)
        {
            _parentDirectory = parentDirectory;
            _dic = dic;
        }

        public override DateTime LastModified => _dic.LastModifed;
        public override bool Exist => true;
        protected override void DeleteImpl() 
            => _parentDirectory?.InfoObject?.Directories?.Remove(_dic);

        protected override DataDirectory? GetInfo(string path) => _dic;

        public override IDirectory GetDirectory(string name) => throw new NotImplementedException();

        public override IEnumerable<IDirectory> Directories
            => _dic.Directories.Select(d => new InMemoryDirectory(this, Path.Combine(OriginalPath, d.Name), d.Name, d));

        public override IEnumerable<IFile> Files
            => _dic.Files.Select(f => new InMemoryFile(this, Path.Combine(OriginalPath, f.Name), f.Name));
        public override IFile GetFile(string name) => throw new NotImplementedException();

        public override IDirectory MoveTo(string location) => throw new NotImplementedException();

                internal DataFile GetOrAddFile(string name);
        internal void Remove(DataFile file);
    }
}