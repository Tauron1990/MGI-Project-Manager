using System;
using JetBrains.Annotations;

namespace Tauron.Application.Files.VirtualFiles.Core
{
    [PublicAPI]
    public abstract class FileSystemNodeBase<TInfo> : IFileSystemNode
        where TInfo : class
    {
        private Lazy<TInfo?> _infoObject;
        private Func<IDirectory?>? _parentDirectory;
        private IDirectory? _parentDirectoryInstance;

        protected FileSystemNodeBase(Func<IDirectory?> parentDirectory, string originalPath, string name)
        {
            _parentDirectory = parentDirectory;
            OriginalPath = Argument.NotNull(originalPath, nameof(originalPath));
            Name = Argument.NotNull(name, nameof(name));
            _infoObject = new Lazy<TInfo?>(() => GetInfo(OriginalPath));
        }

        protected TInfo? InfoObject => _infoObject.Value;
        public abstract DateTime LastModified { get; }

        public IDirectory? ParentDirectory
        {
            get
            {
                if (_parentDirectoryInstance != null) return _parentDirectoryInstance;

                _parentDirectoryInstance = _parentDirectory?.Invoke();
                _parentDirectory = null;

                return _parentDirectoryInstance;
            }
        }

        public void Delete()
        {
            if (Exist)
                DeleteImpl();
        }

        public string Name { get; protected set; }

        public bool IsDirectory { get; private set; }

        public string OriginalPath { get; private set; }

        public abstract bool Exist { get; }

        protected abstract void DeleteImpl();

        protected abstract TInfo? GetInfo([NotNull] string path);

        protected virtual void Reset(string path, IDirectory? parent)
        {
            _parentDirectory = () => parent;
            OriginalPath = Argument.NotNull(path, nameof(path));
            _infoObject = new Lazy<TInfo?>(() => GetInfo(OriginalPath));
        }
    }
}