using System;
using System.Linq;
using System.Threading.Tasks;
using IISServiceManager.Contratcs;
using Microsoft.Web.Administration;

namespace IISServiceManager.Processing
{
    public class IisContainer : IDisposable
    {
        private readonly ServerManager _serverManager = new ServerManager();
        
        public void Dispose() 
            => ((IDisposable) _serverManager)?.Dispose();

        public Task<Site> FindSite(string id) 
            => Task.FromResult(_serverManager.Sites.FirstOrDefault(s => s.Name == id));

        public Task<ObjectState> StopSite(Site site) 
            => Task.FromResult(site?.Stop() ?? ObjectState.Unknown);

        public Task<ObjectState> StartSite(Site site) 
            => Task.FromResult(site?.Start() ?? ObjectState.Unknown);

        public async Task<ObjectState> CreateSite(string binaryPath, IWebService service, IWebServiceCluster cluster,
            ILog log)
        {
            await cluster.PrepareServer(_serverManager, log);
            var pool = await cluster.GetAppPool(_serverManager.ApplicationPools);

            var site = _serverManager.Sites.Add(service.Id, binaryPath, cluster.Config.Ports[service.Id]);

            site.ApplicationDefaults.ApplicationPoolName = pool.Name;
        }
    }
}