﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Anotar.Serilog;
using Catel.Collections;
using Catel.Threading;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Tauron.Application.Deployment.AutoUpload.Models.Github;
using Tauron.Application.ToolUI.Core;

namespace Tauron.Application.Deployment.AutoUpload.Models.Core
{
    public sealed class Settings : ObservableObject
    {
        private readonly AppInfo _info;
        private static readonly string[] SettingFiles = {"conig.1.json", "conig.2.json", "conig.3.json"};

        private readonly AsyncLock _asyncLock = new AsyncLock();

        private int _version;

        private Settings(AppInfo info) 
            => _info = info;

        public IList<string> KnowenRepositorys { get; } = new FastObservableCollection<string>();

        public IList<RegistratedRepository> RegistratedRepositories { get; } = new FastObservableCollection<RegistratedRepository>();

        public IList<VersionRepository> VersionRepositories { get; } = new FastObservableCollection<VersionRepository>();

        public string UserName { get; set; } = string.Empty;

        public string EMailAdress { get; set; } = string.Empty;

        public DateTimeOffset UserWhen { get; set; }

        public async Task AddRepoAndSave(string repo)
        {
            if (KnowenRepositorys.Contains(repo)) return;

            KnowenRepositorys.Add(repo);
            await Save();
        }

        public async Task AddProjecktAndSave(RegistratedRepository repository)
        {
            RegistratedRepositories.Add(repository);
            await Save();
        }

        public async Task AddProjecktsAndSave(IEnumerable<RegistratedRepository> repositorys)
        {
            RegistratedRepositories.AddRange(repositorys);
            await Save();
        }

        public async Task RemoveProjecktAndSave(RegistratedRepository repository)
        {
            RegistratedRepositories.Remove(repository);
            await Save();
        }

        public async Task AddVersionRepoAndSave(VersionRepository versionRepository)
        {
            VersionRepositories.Add(versionRepository);
            await Save();
        }

        public async Task RemoveVersionRepoAndSave(VersionRepository versionRepository)
        {
            VersionRepositories.Remove(versionRepository);
            await Save();
        }

        public static Settings Create(AppInfo info)
        {
            LogTo.Information("Read Settings File");
            var components = SettingFiles
               .Select(s => Path.Combine(info.SettingsDic, s))
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

            LogTo.Information("Find Last Setting Version");
            foreach (var settingsComponent in components)
            {
                target ??= settingsComponent;

                if (target?.Version < settingsComponent?.Version)
                    target = settingsComponent;
            }

            var settings = new Settings(info);

            LogTo.Information("Read Settings");
            if (target != null)
                settings.ReadFromComponent(target);

            return settings;
        }

        public async Task Save()
        {
            LogTo.Information("Saving Settings");
            using (await _asyncLock.LockAsync())
            {
                _version++;

                foreach (var settingFile in SettingFiles)
                {
                    if (!Directory.Exists(_info.SettingsDic))
                        Directory.CreateDirectory(_info.SettingsDic);

                    var filePath = Path.Combine(_info.SettingsDic, settingFile);

                    try
                    {
                        LogTo.Information("Saving Settings File: {Name}", settingFile);

                        if (File.Exists(filePath))
                            File.Delete(filePath);

                        var json = JsonConvert.SerializeObject(new SettingsComponent(this), Formatting.Indented);
                        await File.WriteAllTextAsync(filePath, json);
                    }
                    catch(Exception e)
                    {
                        LogTo.Warning(e, "Error while Saving Settings");
                    }
                }
            }
        }

        private void ReadFromComponent(SettingsComponent component)
        {
            _version = component.Version;
            if (component.EMailAdress != null) EMailAdress = component.EMailAdress;
            if (component.UserName    != null) UserName = component.UserName;
            UserWhen = component.UserWhen;

            RegistratedRepositories.Clear();
            KnowenRepositorys.Clear();

            if (component.KnowenRepositorys != null)
                KnowenRepositorys.AddRange(component.KnowenRepositorys);

            if (component.RegistratedRepository != null)
            {
                foreach (var repositoryComponent in component.RegistratedRepository)
                {
                    RegistratedRepositories.Add(new RegistratedRepository(repositoryComponent.Id,
                        repositoryComponent.BranchName     ?? string.Empty,
                        repositoryComponent.ProjectName    ?? string.Empty,
                        repositoryComponent.RepositoryName ?? string.Empty,
                        repositoryComponent.RealPath       ?? string.Empty));
                }
            }

            if (component.VersionRepositorys != null)
            {
                foreach (var repositoryComponent in component.VersionRepositorys)
                    VersionRepositories.Add(new VersionRepository(repositoryComponent.Name ?? string.Empty, repositoryComponent.Path ?? string.Empty, repositoryComponent.Id));
            }
        }

        public class VersionRepositoryComponent
        {
            public VersionRepositoryComponent(string? name, string? path, long id)
            {
                Name = name;
                Path = path;
                Id = id;
            }

            public string? Name { get; set; }

            public string? Path { get; set; }

            public long Id { get; set; }
        }

        [UsedImplicitly]
        public class RegistratedRepositoryComponent
        {
            public RegistratedRepositoryComponent()
            {
            }

            public RegistratedRepositoryComponent(long id, string branchName, string projectName, string reporitoryName, string? realPath)
            {
                Id = id;
                BranchName = branchName;
                ProjectName = projectName;
                RepositoryName = reporitoryName;
                RealPath = realPath;
            }

            public long Id { get; set; }

            public string? BranchName { get; set; }

            public string? ProjectName { get; set; }

            public string? RepositoryName { get; set; }

            public string? RealPath { get; set; }
        }

        [UsedImplicitly]
        public class SettingsComponent
        {
            public SettingsComponent()
            {
            }

            public SettingsComponent(Settings settings)
            {
                Version = settings._version;
                KnowenRepositorys = new List<string>(settings.KnowenRepositorys);
                RegistratedRepository = new List<RegistratedRepositoryComponent>();
                VersionRepositorys = new List<VersionRepositoryComponent>();

                UserName = settings.UserName;
                EMailAdress = settings.EMailAdress;
                UserWhen = settings.UserWhen;

                foreach (var repository in settings.RegistratedRepositories)
                    RegistratedRepository.Add(new RegistratedRepositoryComponent(repository.Id, repository.BranchName, repository.ProjectName, repository.RepositoryName, repository.RealPath));

                foreach (var repository in settings.VersionRepositories)
                    VersionRepositorys.Add(new VersionRepositoryComponent(repository.Name, repository.RealPath, repository.Id));
            }

            public int Version { get; set; }

            public List<string>? KnowenRepositorys { get; set; }

            public List<RegistratedRepositoryComponent>? RegistratedRepository { get; set; }

            public List<VersionRepositoryComponent>? VersionRepositorys { get; set; }

            public string? UserName { get; set; }

            public string? EMailAdress { get; set; }

            public DateTimeOffset UserWhen { get; set; }
        }
    }
}