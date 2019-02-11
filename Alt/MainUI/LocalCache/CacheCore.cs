using System;
using System.Collections.Generic;

namespace Tauron.Application.MgiProjectManager.LocalCache
{
    [Serializable]
    public class CacheCore
    {
        public CacheCore()
        {
            CacheItems = new List<ICacheItem>();
            Actions = new List<ICacheAction>();
        }

        public List<ICacheItem> CacheItems { get; }

        public List<ICacheAction> Actions { get; }
    }
}