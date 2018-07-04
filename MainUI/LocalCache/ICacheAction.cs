using System;
using System.Collections.Generic;
using Tauron.Application.ProjectManager.UI;

namespace Tauron.Application.MgiProjectManager.LocalCache
{
    public interface ICacheAction
    {
        Type IdentiferType { get; }
        void Try(List<ICacheItem> items, ClientObjectBase client);
    }
}