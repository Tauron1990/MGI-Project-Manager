using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ServiceManager.Core
{
    public sealed class RunningServices
    {
        public static async Task<RunningServices> Read(string path) 
            => File.Exists(path) ? new RunningServices() : JsonConvert.DeserializeObject<RunningServices>(await File.ReadAllTextAsync(path));

        public static async Task Write(RunningServices runningServices, string path) 
            => await File.WriteAllTextAsync(path, JsonConvert.SerializeObject(runningServices));
    }
}