using System;
using Tauron.Application.ProjectManager.Generic;

namespace Tauron.Application.MgiProjectManager.LocalCache
{
    public interface ICacheBase
    {
        void SetServiceManager(ServiceManager manager);

        (ICacheItem, ICacheAction) Fail(Delegate del, object parm);
    }
}