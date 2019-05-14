using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Web.Administration;

namespace IISServiceManager.Contratcs
{
    public interface IWebServiceCluster
    {
        string Name { get; }

        string Id { get; }

        IClusterConfig Config { get; }

        Task<IEnumerable<IWebService>> GetServices();

        Task<object> CheckPrerequisites();

        Task<bool> Build(string repoLocation, string targetPath, IWebService service, ILog log);

        Task PrepareServer(ServerManager manager, ILog log);

        Task<ApplicationPool> GetAppPool(ApplicationPoolCollection applicationPools);
    }
}