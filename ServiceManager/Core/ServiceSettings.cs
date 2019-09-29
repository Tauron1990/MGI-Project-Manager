using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ServiceManager.Services;

namespace ServiceManager.Core
{
    public sealed class ServiceSettings
    {
        public string Url { get; set; }

        public string ApiKey { get; set; }

        public List<RunningService> RunningServices { get; set; } = new List<RunningService>();

        public static ServiceSettings Read(string path)
            => !File.Exists(path) ? new ServiceSettings() : JsonConvert.DeserializeObject<ServiceSettings>(File.ReadAllText(path));

        public static async Task Write(ServiceSettings serviceSettings, string path)
            => await File.WriteAllTextAsync(path, JsonConvert.SerializeObject(serviceSettings));
    }
}