using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Functional;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Nito.AsyncEx;
using Tauron.Application.Deployment.AutoUpload.Github;

namespace Tauron.Application.Deployment.AutoUpload.Core
{
    public sealed class Settings
    {
        [UsedImplicitly]
        public class RegistratedRepositoryComponent
        {
            public long Id { get; set; }

            public string? BranchName { get; set; }

            public string? ProjectName { get; set; }

            public RegistratedRepositoryComponent()
            {
                
            }

            public RegistratedRepositoryComponent(long id, string branchName, string projectName)
            {
                Id = id;
                BranchName = branchName;
                ProjectName = projectName;
            }
        }

        [UsedImplicitly]
        public class SettingsComponent
        {
            public int Version { get; set; }

            public List<string>? KnowenRepositorys { get; set; }

            public List<RegistratedRepositoryComponent>? RegistratedRepository { get; set; }

            public SettingsComponent()
            {
                
            }

            public SettingsComponent(Settings settings)
            {
                Version = settings._version;
                KnowenRepositorys = settings.KnowenRepositorys;
                RegistratedRepository = new List<RegistratedRepositoryComponent>();

                foreach (var repository in settings.RegistratedRepositories) 
                    RegistratedRepository.Add(new RegistratedRepositoryComponent(repository.Id, repository.BranchName, repository.ProjectName));
            }
        }

        private static readonly string[] SettingFiles = {"conig.1.json", "conig.2.json", "conig.3.json"};

        private static readonly string SettingsDic = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Tauron", "Tauron.Application.Deployment.AutoUpload");

        private readonly AsyncLock _asyncLock = new AsyncLock();

        private int _version;

        public List<string> KnowenRepositorys { get; } = new List<string>();

        public List<RegistratedRepository> RegistratedRepositories { get; } = new List<RegistratedRepository>();

        private Settings()
        {
            
        }

        public async Task AddRepoAndSave(string repo)
        {
            if(KnowenRepositorys.Contains(repo)) return;

            KnowenRepositorys.Add(repo);
            await Save();
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

        private async Task Save()
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
            KnowenRepositorys.Clear();
            
            if(component.KnowenRepositorys != null)
                KnowenRepositorys.AddRange(component.KnowenRepositorys);

            if (component.RegistratedRepository != null)
            {
                foreach (var repositoryComponent in component.RegistratedRepository) 
                    RegistratedRepositories.Add(new RegistratedRepository(repositoryComponent.Id,
                        repositoryComponent.BranchName ?? string.Empty, 
                        repositoryComponent.ProjectName ?? string.Empty));
            }
        }
    }
}