using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using ServiceManager.Installation.Core;
using Tauron.CQRS.Common;

namespace ServiceManager.Installation.Tasks
{
    public class UpdateFilesTask : InstallerTask
    {
        private readonly ILogger<UpdateFilesTask> _logger;

        public override string Title => "Aktualisieren";

        public UpdateFilesTask(ILogger<UpdateFilesTask> logger) 
            => _logger = logger;

        public override async Task<string> RunInstall(InstallerContext context)
        {
            Content = "Einstellungen werden Übernommen";

            if (!context.MetaData.TryGetTypedValue(MetaKeys.TempLocation, out string tempPath))
                return "Temporärer Pfad nich angegeben";

            var serviceSettings = Path.Combine(tempPath, InstallerContext.ServiceSettingsFileName);
            var appSettings = Path.Combine(tempPath, InstallerContext.AppSettingsFileName);

            var result = await TryMerge(await TryCreateConfiguration(serviceSettings), Path.Combine(context.InstalledPath, InstallerContext.ServiceSettingsFileName), serviceSettings);
            if (result)
                return "Fehler beim mergen der Service Settings";

            result = await TryMerge(await TryCreateConfiguration(appSettings), Path.Combine(context.InstalledPath, InstallerContext.AppSettingsFileName), appSettings);
            if (result)
                return "Fehler beim mergen der App Settings";

            Content = "Alte Daten Löschen";

            foreach (var info in new DirectoryInfo(context.InstalledPath).EnumerateFileSystemInfos())
            {
                switch (info)
                {
                    case DirectoryInfo directoryInfo:
                        directoryInfo.Delete(true);
                        break;
                    case FileInfo fileInfo:
                        fileInfo.Delete();
                        break;
                }
            }

            Content = "Daten Übertragen";

            DirectoryCopy(tempPath, context.InstalledPath);

            return null;
        }

        private async Task<bool> TryMerge(JToken root, string source, string rootPath)
        {
            if (!File.Exists(source)) return true;

            try
            {
                var sourceToken = JToken.Parse(await File.ReadAllTextAsync(source));

                ((JContainer)root).Merge(sourceToken, new JsonMergeSettings
                                                      {
                                                          MergeArrayHandling = MergeArrayHandling.Union, 
                                                          MergeNullValueHandling = MergeNullValueHandling.Ignore, 
                                                          PropertyNameComparison = StringComparison.Ordinal
                                                      });

                await File.WriteAllTextAsync(rootPath, root.ToString());

                return true;
            }
            catch(Exception e)
            {
                _logger.LogWarning(e, "Error while Merging Settings");

                return false;
            }
        }

        private static void DirectoryCopy(string sourceDirName, string destDirName)
        {
            // Get the subdirectories for the specified directory.
            var dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                  + sourceDirName);
            }

            var dirs = dir.GetDirectories();
            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destDirName)) 
                Directory.CreateDirectory(destDirName);

            // Get the files in the directory and copy them to the new location.
            var files = dir.GetFiles();
            foreach (var file in files)
            {
                var temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
            }

            // If copying subdirectories, copy them and their contents to new location.
            foreach (var subdir in dirs)
            {
                string temppath = Path.Combine(destDirName, subdir.Name);
                DirectoryCopy(subdir.FullName, temppath);
            }

        }

        private static async Task<JToken> TryCreateConfiguration(string name)
            => File.Exists(name) ? new JObject() : JToken.Parse(await File.ReadAllTextAsync(name));

    }
}