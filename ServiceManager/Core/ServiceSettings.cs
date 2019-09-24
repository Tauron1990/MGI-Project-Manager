using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ServiceManager.Core
{
    public sealed class ServiceSettings
    {
        public string Url { get; set; }

        public string ApiKey { get; set; }

        public static ServiceSettings Read(string path) 
            => !File.Exists(path) ? new ServiceSettings() : JsonConvert.DeserializeObject<ServiceSettings>(File.ReadAllText(path));

        public static async Task Write(ServiceSettings serviceSettings, string path) 
            => await File.WriteAllTextAsync(path, JsonConvert.SerializeObject(serviceSettings));
    }
}