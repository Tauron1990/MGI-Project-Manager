﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IISServiceManager.Contratcs;
using IISServiceManager.MgiProjectManager.Resources;
using Microsoft.Web.Administration;

namespace IISServiceManager.MgiProjectManager
{
    public class MgiServiceCluster : IWebServiceCluster
    {
        public string Name => Strings.MgiServiceClusterName;
         
        public string Id { get; } = nameof(MgiServiceCluster);

        public IClusterConfig Config { get; }

        public Task<IEnumerable<IWebService>> GetServices() => Task.FromResult(Enumerable.Empty<IWebService>());

        public Task<object> CheckPrerequisites() => Task.FromResult<object>("https://aka.ms/dotnet-download");
        public Task<bool> Build(string repoLocation, string targetPath, IWebService service, ILog log)
        {
            throw new System.NotImplementedException();
        }

        public Task PrepareServer(ServerManager manager)
        {
            throw new System.NotImplementedException();
        }
    }
}