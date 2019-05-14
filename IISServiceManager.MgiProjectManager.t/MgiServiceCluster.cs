using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IISServiceManager.Contratcs;
using IISServiceManager.MgiProjectManager.Resources;

namespace IISServiceManager.MgiProjectManager
{
    public class MgiServiceCluster : IWebServiceCluster
    {
        public string Name => Strings.MgiServiceClusterName;
         
        public string Id { get; } = nameof(MgiServiceCluster);

        public IClusterConfig Config { get; }

        public Task<IEnumerable<IWebService>> GetServices() => Task.FromResult(Enumerable.Empty<IWebService>());

        public Task<object> CheckPrerequisites() => Task.FromResult<object>("https://aka.ms/dotnet-download");
    }
}