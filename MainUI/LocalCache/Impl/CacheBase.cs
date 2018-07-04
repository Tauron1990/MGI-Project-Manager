using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Tauron.Application.ProjectManager.Generic;
using Tauron.Application.ProjectManager.UI;

namespace Tauron.Application.MgiProjectManager.LocalCache.Impl
{
    public abstract class CacheBase : ICacheBase
    {
        [Serializable]
        public abstract class CacheItemBase<TItem> : ICacheItem
        {
            public abstract Type IdentiferType { get; }
            public object Content { get; }

            public TItem Item => (TItem) Content;

            protected CacheItemBase(TItem content) => Content = content;
        }

        [Serializable]
        public class CacheActionBase : ICacheAction
        {
            private MethodInfo _method;

            public Type IdentiferType { get; }

            public CacheActionBase(Delegate del)
            {
                _method = del.Method;
                var length = _method.GetParameters().Length;
                if(length > 1)
                    throw new ArgumentException("Only Method with Zero or One Parameter are Supported");

                IdentiferType = del.Target.GetType().GetInterfaces().Single(i => i.Name != "ITypedClientHelperBase" && i.Name != "IClientHelperBase");
            }

            public void Try(List<ICacheItem> items, ClientObjectBase client)
            {
                var length = _method.GetParameters().Length;
                if (length == 0)
                    _method.Invoke(client.CommunicationObject);
                else
                {
                    
                }
            }
        }

        public virtual void SetServiceManager(ServiceManager manager)
        {
            
        }

        public abstract (ICacheItem, ICacheAction) Fail(Delegate del, object parm);
    }
}