using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Tauron.Application.SoftwareRepo.Data;

namespace Tauron.Application.SoftwareRepo
{
    public class SoftwareRepository
    {
        private const string FileName = "Apps.json";

        private readonly string _path;

        public ApplicationList ApplicationList { get; private set; } = new ApplicationList(ImmutableList<ApplicationEntry>.Empty);

        private SoftwareRepository(string path) 
            => _path = path;

        private async Task Init()
        {
            var compledPath = GetFullPath();

            if(!File.Exists(compledPath))
                throw new InvalidOperationException("Apps File not found");

            ApplicationList = JsonConvert.DeserializeObject<ApplicationList>(await File.ReadAllTextAsync(compledPath));
        }

        private async Task InitNew()
        {
            var compledPath = GetFullPath();

            if (!File.Exists(compledPath))
                File.Delete(compledPath);

            await File.WriteAllTextAsync(compledPath, JsonConvert.SerializeObject(ApplicationList));
        }

        private string GetFullPath()
            => Path.Combine(_path, FileName);

        public static async Task<SoftwareRepository> Create(string path)
        {
            var temp = new SoftwareRepository(path);
            await temp.InitNew();
            return temp;
        }

        public static async Task<SoftwareRepository> Read(string path)
        {
            var temp = new SoftwareRepository(path);
            await temp.Init();
            return temp;
        }

        public static bool IsValid(string path)
            => File.Exists(Path.Combine(path, FileName));

        public async Task Save() 
            => await File.WriteAllTextAsync(GetFullPath(), JsonConvert.SerializeObject(ApplicationList));

        public async Task ChangeName(string? name = null, string? description = null)
        {
            if (!string.IsNullOrWhiteSpace(name))
                ApplicationList.Name = name;
            if (!string.IsNullOrWhiteSpace(description))
                ApplicationList.Description = description;

            await Save();
        }

        public object CreateBackup() => new ApplicationList(ApplicationList);

        public void Revert(object backup)
        {
            if (backup is ApplicationList applicationList)
                ApplicationList = new ApplicationList(applicationList);
            else
                throw new InvalidOperationException("Falsches backup übergeben");
        }

        public long Contains(string name) 
            => ApplicationList.ApplicationEntries.Find(ae => ae.Name == name)?.Id ?? -1;

        public void AddApplication(string name, long id, string url, Version version)
        {
            if(Contains(name) != -1)
                throw new InvalidOperationException("Der Eintrag Existiert schon");

            ApplicationList.ApplicationEntries =
                ApplicationList.ApplicationEntries.Add(
                    new ApplicationEntry(name, version, id, ImmutableList<DownloadEntry>.Empty.Add(new DownloadEntry(version, url))));
        }

        public void UpdateApplication(long id, Version version, string url)
        {
            var entry = ApplicationList.ApplicationEntries.Find(ae => ae.Id == id);
            if(entry == null)
                throw new InvalidOperationException("Eintrag nicht gefunden");

            entry.Last = version;
            entry.Downloads = entry.Downloads.Add(new DownloadEntry(version, url));
        }
    }
}