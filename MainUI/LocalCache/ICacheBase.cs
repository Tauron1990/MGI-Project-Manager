using System;
using System.Collections.Generic;
using Tauron.Application.ProjectManager.Generic;

namespace Tauron.Application.MgiProjectManager.LocalCache
{
    public interface ICacheBase
    {
        void SetServiceManager(ServiceManager manager);

        (IEnumerable<ICacheItem>, ICacheAction)? Fail(Delegate del, object[] parms);
    }
}