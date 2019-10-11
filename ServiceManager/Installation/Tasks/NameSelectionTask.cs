using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ServiceManager.Core;
using ServiceManager.Installation.Core;
using ServiceManager.Installation.Tasks.Ui;

namespace ServiceManager.Installation.Tasks
{
    public sealed class NameSelectionTask : InstallerTask
    {
        private readonly ILogger<NameSelectionTask> _logger;
        private readonly ServiceSettings _serviceSettings;
        private NameSelectionModel _nameSelectionModel;

        public NameSelectionTask(ILogger<NameSelectionTask> logger, ServiceSettings serviceSettings)
        {
            _logger = logger;
            _serviceSettings = serviceSettings;
        }

        public override string Title => "Service Name";

        public override Task Prepare(InstallerContext context)
        {
            _nameSelectionModel = context.ServiceScope.ServiceProvider.GetRequiredService<NameSelectionModel>();
            Content = context.ServiceScope.ServiceProvider.GetRequiredService<NameSelection>();

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
            }
            else
            {
                _logger.LogInformation("Awaitng Name Selection");

                await _nameSelectionModel.Wait();
                name = _nameSelectionModel.NameText;
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                _logger.LogWarning("No Name Selected");
                return "Kein korrekter name wurde angegeben";
            }

            if (_serviceSettings.RunningServices.Select(rs => rs.Name).Contains(name))
                return "Name schon Vergeben";

            context.ServiceName = name;

            _logger.LogInformation($"{context.ServiceName}: Try Find Exe Name");
            var exeName = GetExeName(context.PackageArchive);

            if (string.IsNullOrEmpty(exeName))
            {
                _logger.LogInformation($"{context.ServiceName}: Search for Exe in Package");
                exeName = context.PackageArchive.Entries.FirstOrDefault(e => e.Name.EndsWith(".exe"))?.FullName;

                if (string.IsNullOrEmpty(exeName))
                {
                    _logger.LogWarning($"{context.ServiceName}: No Exe Found");
                    return "Keine Ausfürbare Datei gefunden";
                }
            }

            _logger.LogInformation($"{context.ServiceName}: Exe Found: {exeName}");
            context.ExeName = exeName;

            return null;
        }

        private static string GetName(ZipArchive zipArchive) 
            => GetConfiguration(zipArchive)?
               .GetValue<string>("ServiceName");

        private static string GetExeName(ZipArchive zipArchive) 
            => GetConfiguration(zipArchive)?.GetValue<string>("ExeName");

        private static IConfiguration GetConfiguration(ZipArchive zipArchive)
        {
            var file = zipArchive.GetEntry(InstallerContext.ServiceSettingsFileName);
            if (file == null) return null;
            var mem = new MemoryStream();
            using (var fileStream = file.Open()) fileStream.CopyTo(mem);

            mem.Position = 0;

            return new ConfigurationBuilder()
                .AddJsonStream(mem)
                //.AddJsonFile(new ZipFileProvider(zipArchive), InstallerContext.SettingsFileName, false, false)
                .Build();
        }

        //private sealed class ZipFileProvider : IFileProvider
        //{
        //    private readonly ZipArchive _archive;
            
        //    public ZipFileProvider(ZipArchive archive) => _archive = archive;

        //    public IFileInfo GetFileInfo(string subpath) => new FileInfo(subpath, _archive);

        //    public IDirectoryContents GetDirectoryContents(string subpath) => new DirectoryContents(subpath, _archive);

        //    public IChangeToken Watch(string filter) => new ChangeToken();

        //    private class DisposeNull : IDisposable
        //    {
        //        public void Dispose()
        //        {
        //        }
        //    }

        //    private class ChangeToken : IChangeToken
        //    {
        //        public IDisposable RegisterChangeCallback(Action<object> callback, object state) => new DisposeNull();

        //        public bool HasChanged => false;
        //        public bool ActiveChangeCallbacks => false;
        //    }

        //    private class FileInfo : IFileInfo
        //    {
        //        private readonly ZipArchive _zip;

        //        public FileInfo(string path, ZipArchive zip)
        //        {
        //            PhysicalPath = path;
        //            _zip = zip;
        //        }

        //        public Stream CreateReadStream() => _zip.GetEntry(PhysicalPath)?.Open();

        //        public bool Exists => Entry != null;

        //        public long Length => Entry?.Length ?? 0;

        //        public string PhysicalPath { get; }

        //        public string Name => Path.GetFileName(PhysicalPath);

        //        public DateTimeOffset LastModified => Entry?.LastWriteTime ?? DateTimeOffset.MinValue;

        //        public bool IsDirectory => Path.HasExtension(PhysicalPath);

        //        [CanBeNull]
        //        private ZipArchiveEntry Entry => _zip.GetEntry(PhysicalPath);
        //    }

        //    private class DirectoryContents : IDirectoryContents
        //    {
        //        private readonly string _root;
        //        private readonly ZipArchive _zip;
        //        private readonly int _segments;

        //        public DirectoryContents(string root, ZipArchive zip)
        //        {
        //            _root = root;
        //            _zip = zip;
        //            _segments = root.Split('\\').Length;
        //        }

        //        public IEnumerator<IFileInfo> GetEnumerator() 
        //            => _zip.Entries
        //               .Where(e => e.FullName.StartsWith(_root) && e.FullName.Split('\\').Length == _segments - 1)
        //               .Select(e => new FileInfo(e.FullName, _zip)).Cast<IFileInfo>()
        //               .GetEnumerator();

        //        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        //        public bool Exists => true;
        //    }
        //}
    }
}