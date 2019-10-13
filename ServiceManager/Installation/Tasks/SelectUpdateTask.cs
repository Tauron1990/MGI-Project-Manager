using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Extensions.Logging;
using ServiceManager.Installation.Core;

namespace ServiceManager.Installation.Tasks
{
    public sealed class SelectUpdateTask : InstallerTask
    {
        private readonly MainWindow _mainWindow;
        private readonly ILogger<SelectUpdateTask> _logger;
        private ZipArchive _zipArchive;

        public override string Title => "Datei Wählen";

        public SelectUpdateTask(MainWindow mainWindow, ILogger<SelectUpdateTask> logger)
        {
            _mainWindow = mainWindow;
            _logger = logger;
        }

        public override Task Prepare(InstallerContext context)
        {
            Content = "Update Packet Wählen";

            return base.Prepare(context);
        }

        public override Task<string> RunInstall(InstallerContext context)
        {
            var service = context.CreateRunningService();

            using var diag = new OpenFileDialog { AutoUpgradeEnabled = true };


            if (diag.ShowDialog(new Win32Proxy(_mainWindow)) != DialogResult.OK)
            {
                _logger.LogWarning($"{service.Name}: Update File not Selected");
                return Task.FromResult("Es wurde Keine Datei fürs Update gewählt.");
            }

            var path = diag.FileName;
            try
            {
                var archive = new ZipArchive(new FileStream(path, FileMode.Open));

                context.MetaData[MetaKeys.UpdateFile] = path;
                context.MetaData[MetaKeys.ArchiveFile] = archive;

                _zipArchive = archive;
            }
            catch
            {
                _logger.LogWarning($"{service.Name}: Error Open Update File");
                return Task.FromResult("Die Datei konnte nicht geöffnet werden");
            }

            return Task.FromResult <string>(null);
        }

        public override void Dispose() 
            => _zipArchive?.Dispose();
    }
}