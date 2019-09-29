using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using ServiceManager.Installation.Core;
using ServiceManager.Installation.Tasks.Ui;

namespace ServiceManager.Installation.Tasks
{
    public sealed class NameSelectionTask : InstallerTask
    {
        private readonly ILogger<NameSelectionTask> _logger;
        private object _content;
        private NameSelectionModel _nameSelectionModel;

        public NameSelectionTask(ILogger<NameSelectionTask> logger) => _logger = logger;

        public override object Content => _content;

        public override string Title => "Service Name";

        public override Task Prepare(InstallerContext context)
        {
            _nameSelectionModel = context.ServiceScope.ServiceProvider.GetRequiredService<NameSelectionModel>();
            _content = context.ServiceScope.ServiceProvider.GetRequiredService<NameSelection>();

            return Task.CompletedTask;
        }

        public override async Task<string> RunInstall(InstallerContext context)
        {
            var zip = context.PackageArchive;
            if (zip == null)
            {
                _logger.LogWarning($"{Path.GetFileName(context.PackagePath)} is no Zip File");
                return "Datei ist keine Zip Datei";
            }

            var name = GetName(zip);

            if (!string.IsNullOrWhiteSpace(name))
            {
                _logger.LogInformation($"Service Name in AppSettings Found: {name}");
                context.ServiceName = name;
                return null;
            }

            _logger.LogInformation("Awaitng Name Selection");

            await _nameSelectionModel.Wait();

            name = _nameSelectionModel.NameText;

            if (string.IsNullOrWhiteSpace(name))
            {
                _logger.LogWarning("No Name Selected");
                return "Kein korrekter name wurde angegeben";
            }

            context.ServiceName = name;

            return null;
        }

        private static string GetName(ZipArchive zipArchive) 
            => new ConfigurationBuilder()
               .AddJsonFile(new ZipFileProvider(zipArchive), "appsettings.json", false, false)
               .Build()
               .GetValue<string>("ServiceName");

        private sealed class ZipFileProvider : IFileProvider
        {
            private readonly ZipArchive _archive;
            
            public ZipFileProvider(ZipArchive archive) => _archive = archive;

            public IFileInfo GetFileInfo(string subpath) => new FileInfo(subpath, _archive);

            public IDirectoryContents GetDirectoryContents(string subpath) => new DirectoryContents(subpath, _archive);

            public IChangeToken Watch(string filter) => new ChangeToken();

            private class DisposeNull : IDisposable
            {
                public void Dispose()
                {
                }
            }

            private class ChangeToken : IChangeToken
            {
                public IDisposable RegisterChangeCallback(Action<object> callback, object state) => new DisposeNull();

                public bool HasChanged => false;
                public bool ActiveChangeCallbacks => false;
            }

            private class FileInfo : IFileInfo
            {
                private readonly ZipArchive _zip;

                public FileInfo(string path, ZipArchive zip)
                {
                    PhysicalPath = path;
                    _zip = zip;
                }

                public Stream CreateReadStream() => _zip.GetEntry(PhysicalPath)?.Open();

                public bool Exists => Entry != null;

                public long Length => Entry?.Length ?? 0;

                public string PhysicalPath { get; }

                public string Name => Path.GetFileName(PhysicalPath);

                public DateTimeOffset LastModified => Entry?.LastWriteTime ?? DateTimeOffset.MinValue;

                public bool IsDirectory => Path.HasExtension(PhysicalPath);

                [CanBeNull]
                private ZipArchiveEntry Entry => _zip.GetEntry(PhysicalPath);
            }

            private class DirectoryContents : IDirectoryContents
            {
                private readonly string _root;
                private readonly ZipArchive _zip;
                private readonly int _segments;

                public DirectoryContents(string root, ZipArchive zip)
                {
                    _root = root;
                    _zip = zip;
                    _segments = root.Split('\\').Length;
                }

                public IEnumerator<IFileInfo> GetEnumerator() 
                    => _zip.Entries
                       .Where(e => e.FullName.StartsWith(_root) && e.FullName.Split('\\').Length == _segments - 1)
                       .Select(e => new FileInfo(e.FullName, _zip)).Cast<IFileInfo>()
                       .GetEnumerator();

                IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

                public bool Exists => true;
            }
        }
    }
}