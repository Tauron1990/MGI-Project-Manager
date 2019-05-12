using System.Collections.Generic;
using System.Threading.Tasks;

namespace IISServiceManager.Contratcs
{
    public interface IWebServiceCluster
    {
        string Name { get; }

        string Id { get; }

        Task<IEnumerable<IWebService>> GetServices();

        Task<object> CheckPrerequisites();
    }
}