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
        {
            if (site == null)
                return Task.FromResult(ObjectState.Unknown);

            var pool = _serverManager.ApplicationPools[site.ApplicationDefaults.ApplicationPoolName];
            if (pool.State == ObjectState.Started) return Task.FromResult(site.Start());

            var result = pool.Start();
            if (result != ObjectState.Started || result != ObjectState.Starting)
                return Task.FromResult(result);

            return Task.FromResult(site.Start());
        }

        public async Task<ObjectState> CreateSite(string binaryPath, IWebService service, IWebServiceCluster cluster,
            ILog log)
        {
            log.WriteLine("Prepare Server");
            await cluster.PrepareServer(_serverManager, log);

            log.WriteLine("Get AppPool");
            var pool = await cluster.GetAppPool(_serverManager.ApplicationPools);

            log.WriteLine($"Configurate Web Service... {service.Name}");
            var site = _serverManager.Sites.Add(service.Id, binaryPath, cluster.Config.Ports[service.Id]);

            site.ApplicationDefaults.ApplicationPoolName = pool.Name;
            site.ServerAutoStart = true;
            _serverManager.CommitChanges();

            log.WriteLine("Start Web Service...");
            return await StartSite(site);
        }

        public Task DeleteSite(Site site)
        {
            _serverManager.Sites.Remove(site);
            _serverManager.CommitChanges();

            return Task.CompletedTask;
        }
    }
}