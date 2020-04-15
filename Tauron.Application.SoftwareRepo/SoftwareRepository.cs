using System;
using System.Collections.Immutable;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Tauron.Application.SoftwareRepo.Data;

namespace Tauron.Application.SoftwareRepo
{
    public class SoftwareRepository
    {
        internal const string FileName = "Apps.json";

        private readonly string _path;

        internal SoftwareRepository(string path) => _path = path;

        public ApplicationList ApplicationList { get; private set; } = new ApplicationList(ImmutableList<ApplicationEntry>.Empty, string.Empty, string.Empty);

        internal async Task Init()
        {
            var compledPath = GetFullPath();

            if (!File.Exists(compledPath))
                throw new InvalidOperationException("Apps File not found");

            ApplicationList = JsonConvert.DeserializeObject<ApplicationList>(await File.ReadAllTextAsync(compledPath));
        }

        internal async Task InitNew()
        {
            var compledPath = GetFullPath();

            if (!File.Exists(compledPath))
                File.Delete(compledPath);

            await File.WriteAllTextAsync(compledPath, JsonConvert.SerializeObject(ApplicationList));
        }

        private string GetFullPath() => Path.Combine(_path, FileName);


        public async Task Save()
        {
            await File.WriteAllTextAsync(GetFullPath(), JsonConvert.SerializeObject(ApplicationList));
        }

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

        public long Get(string name)
        {
            return ApplicationList.ApplicationEntries.Find(ae => ae.Name == name)?.Id ?? -1;
        }

        public void AddApplication(string name, long id, string url, Version version, string originalRepository, string brnachName)
        {
            if (Get(name) != -1)
                throw new InvalidOperationException("Der Eintrag Existiert schon");

            ApplicationList.ApplicationEntries =
                ApplicationList.ApplicationEntries.Add(
                    new ApplicationEntry(name, version, id, ImmutableList<DownloadEntry>.Empty.Add(new DownloadEntry(version, url)), originalRepository, brnachName));
        }

        public void UpdateApplication(long id, Version version, string url)
        {
            var entry = ApplicationList.ApplicationEntries.Find(ae => ae.Id == id);
            if (entry == null)
                throw new InvalidOperationException("Eintrag nicht gefunden");

            entry.Last = version;
            entry.Downloads = entry.Downloads.Add(new DownloadEntry(version, url));
        }
    }
}