using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Functional;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Nito.AsyncEx;

namespace Tauron.Application.Deployment.AutoUpload.Core
{
    public sealed class Settings
    {
        [UsedImplicitly]
        public class SettingsComponent
        {
            public int Version { get; set; }

            public SettingsComponent()
            {
                
            }

            public SettingsComponent(Settings settings) 
                => Version = settings._version;
        }

        private static readonly string[] SettingFiles = {"conig.1.json", "conig.2.json", "conig.3.json"};

        private static readonly string SettingsDic = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Tauron", "Tauron.Application.Deployment.AutoUpload");

        private readonly AsyncLock _asyncLock = new AsyncLock();

        private int _version;

        private Settings()
        {
            
        }

        public static Settings Create()
        {
            var components = SettingFiles
                .Select(s => Path.Combine(SettingsDic, s))
                .Where(File.Exists)
                .Select(s =>
                {
                    try
                    {
                        return JsonConvert.DeserializeObject<SettingsComponent>(File.ReadAllText(s));
                    }
                    catch
                    {
                        return null;
                    }
                })
                .Where(c => c != null)
                .ToArray();

            SettingsComponent? target = null;

            foreach (var settingsComponent in components)
            {
                if (target == null)
                    target = settingsComponent;

                if (target?.Version < settingsComponent?.Version)
                    target = settingsComponent;
            }

            var settings = new Settings();

            if(target != null)
                settings.ReadFromComponent(target);

            return settings;
        }

        public async Task Save()
        {
            await _asyncLock.LockAsync();
            _version++;

            foreach (var settingFile in SettingFiles)
            {
                if (!Directory.Exists(SettingsDic))
                    Directory.CreateDirectory(SettingsDic);

                var filePath = Path.Combine(settingFile);

                try
                {
                    if (File.Exists(filePath))
                        File.Delete(filePath);

                    var json = JsonConvert.SerializeObject(new SettingsComponent(this));
                    await File.WriteAllTextAsync(filePath, json);
                }
                catch
                {
                    // ignored
                }

            }
        }

        private void ReadFromComponent(SettingsComponent component)
        {
            _version = component.Version;
        }
    }
}