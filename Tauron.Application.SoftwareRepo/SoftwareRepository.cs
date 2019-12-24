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

        protected SoftwareRepository(string path) 
            => _path = path;

        private async Task Init()
        {
            var compledPath = Path.Combine(_path, FileName);

            if(!File.Exists(compledPath))
                throw new InvalidOperationException("Apps File not found");

            ApplicationList = JsonConvert.DeserializeObject<ApplicationList>(await File.ReadAllTextAsync(compledPath));
        }

        public static async Task<SoftwareRepository> Read(string path)
        {
            var temp = new SoftwareRepository(path);
            await temp.Init();
            return temp;
        }

        public static bool IsValid(string path)
            => File.Exists(Path.Combine(path, FileName));
    }
}