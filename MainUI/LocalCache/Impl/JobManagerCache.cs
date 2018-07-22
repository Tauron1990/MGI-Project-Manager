using System;
using System.Collections.Generic;
using System.Linq;

namespace Tauron.Application.MgiProjectManager.LocalCache.Impl
{
    public class JobManagerCache : CacheBase, IJobManagerCache
    {
        private readonly string[] _supportedMethods =
        {
            "InsertJob",
            "MarkImportent",
            "StateTransition"
        };

        public override (IEnumerable<ICacheItem>, ICacheAction)? Fail(Delegate del, object[] parm)
        {
            if (!ValidateDelegate(del, _supportedMethods, out _)) return null;

            var useSuffix = false; //methodName == _supportedMethods[2];
            var id = Guid.NewGuid().ToString();

            var action = new CacheActionBase(del, id, useSuffix);
            var items = parm.Select((t, i) => new CacheItemBase(action.IdentiferType, t, id, i));

            return (items, action);
        }
    }
}