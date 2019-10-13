using System;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using ServiceManager.Installation.Core;
using Tauron.CQRS.Common;

namespace ServiceManager.Installation.Tasks
{
    public class ExtractToTempTask : InstallerTask
    {
        private readonly string _tempPath = Path.Combine(AppContext.BaseDirectory, "Temp");

        public override string Title => "Extrahieren";

        public override Task Prepare(InstallerContext context)
        {
            Content = "Update wird Extrahiert";

            return Task.CompletedTask;
        }

        public override Task<string> RunInstall(InstallerContext context)
        {
            if (!context.MetaData.TryGetTypedValue(MetaKeys.ArchiveFile, out ZipArchive zipArchive))
                return Task.FromResult("Zip Erchive nicht gefunden");

            DeleteTemp();

            zipArchive.ExtractToDirectory(_tempPath);
            context.MetaData[MetaKeys.TempLocation] = _tempPath;

            return Task.FromResult<string>(null);
        }

        public override void Dispose() 
            => DeleteTemp();

        private void DeleteTemp()
        {
            if (Directory.Exists(_tempPath))
                Directory.Delete(_tempPath, true);
        }
    }
}