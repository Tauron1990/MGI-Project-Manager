using System;
using Microsoft.Web.Administration;

namespace IISServiceManager.Processing
{
    public class IisContainer : IDisposable
    {
        private readonly ServerManager _serverManager = new ServerManager();



        public void Dispose() 
            => ((IDisposable) _serverManager)?.Dispose();
    }
}