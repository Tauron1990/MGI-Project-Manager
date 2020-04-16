using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Tauron.Application.Files.VirtualFiles.Core;
using Tauron.Application.Files.VirtualFiles.InMemory.Data;

namespace Tauron.Application.Files.VirtualFiles.InMemory
{
    public class InMemoryDirectory : DirectoryBase<DataDirectory>
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

        public override IDirectory GetDirectory(string name)
        {
            var dataDirectory = _dic.Directories.FirstOrDefault(d => d.Name == name);
            if (dataDirectory == null)
            {
                dataDirectory = new DataDirectory(name);
                _dic.Directories.Add(dataDirectory);
            }

            return new InMemoryDirectory(this, Path.Combine(OriginalPath, name), name, dataDirectory);
        }

        public override IEnumerable<IDirectory> Directories
            => _dic.Directories.Select(d => new InMemoryDirectory(this, Path.Combine(OriginalPath, d.Name), d.Name, d));

        public override IEnumerable<IFile> Files
            => _dic.Files.Select(f => new InMemoryFile(this, Path.Combine(OriginalPath, f.Name), f.Name));

        public override IFile GetFile(string name)
            => new InMemoryFile(this, Path.Combine(OriginalPath, name), name);

        public override IDirectory MoveTo(string location)
            => throw new NotSupportedException();

        internal DataFile GetOrAddFile(string name)
        {
            var data = _dic.Files.FirstOrDefault(df => df.Name == name);
            if (data == null)
            {
                data = new DataFile(name);
                _dic.Files.Add(data);
            }

            return data;
        }

        internal void Remove(DataFile? file)
        {
            if(file == null) return;
            _dic.Files.Remove(file);
        }

        public bool ExistFile(string name) 
            => _dic.Files.Any(df => df.Name == name);
    }
}