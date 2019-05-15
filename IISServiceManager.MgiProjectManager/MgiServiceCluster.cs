using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using IISServiceManager.Contratcs;
using IISServiceManager.MgiProjectManager.Resources;
using Microsoft.Web.Administration;

namespace IISServiceManager.MgiProjectManager
{
    public class MgiServiceCluster : IWebServiceCluster
    {
        private readonly XElement _configuration;

        public string Name { get; } = Strings.MgiServiceClusterName;

        public string Id { get; } = nameof(MgiServiceCluster);

        public IClusterConfig Config { get; }

        public MgiServiceCluster()
        {
            _configuration = XElement.Load(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ServiceConfig.xml"));

            Config = new XmlClusterConfig(_configuration);
        }

        public Task<IEnumerable<IWebService>> GetServices()
            => Task.FromResult((
                                   from xElement in _configuration.Element("Services")?.Elements("Service") ?? Enumerable.Empty<XElement>()
                                   select new XmlWebService(xElement)
                               ).Cast<IWebService>());

        public Task<object> CheckPrerequisites() => throw new System.NotImplementedException();

        public Task<bool> Build(string repoLocation, string targetPath, IWebService service, ILog log) => throw new System.NotImplementedException();

        public Task PrepareServer(ServerManager manager, ILog log) => throw new System.NotImplementedException();

        public Task<ApplicationPool> GetAppPool(ApplicationPoolCollection applicationPools) => throw new System.NotImplementedException();
    }
}