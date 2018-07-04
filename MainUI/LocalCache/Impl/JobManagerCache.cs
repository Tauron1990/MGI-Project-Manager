using System;
using Tauron.Application.ProjectManager.Generic;
using Tauron.Application.ProjectManager.UI;

namespace Tauron.Application.MgiProjectManager.LocalCache.Impl
{
    public class JobManagerCache : ICacheBase, IJobManagerCache
    {
        public class JobCacheItem
        {
            
        }

        private ServiceManager _serviceManager;

        public void SetServiceManager(ServiceManager manager)
        {
        }

        public (ICacheItem, ICacheAction) Fail(Delegate del, object parm)
        {
            throw new NotImplementedException();
        }
    }
}