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

        public Task<IEnumerable<IWebService>> GetServices() => Task.FromResult(Enumerable.Empty<IWebService>());

        public Task<object> CheckPrerequisites() => Task.FromResult<object>(null);
    }
}