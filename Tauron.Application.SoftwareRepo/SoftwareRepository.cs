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

        public async Task ChangeName(string? name = null, string? description = null)
        {
            if (!string.IsNullOrWhiteSpace(name))
                ApplicationList.Name = name;
            if (!string.IsNullOrWhiteSpace(description))
                ApplicationList.Description = description;

            await File.WriteAllTextAsync(GetFullPath(), JsonConvert.SerializeObject(ApplicationList));
        }
    }
}