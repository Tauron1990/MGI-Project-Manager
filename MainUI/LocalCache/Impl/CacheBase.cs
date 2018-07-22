using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using NLog;
using Tauron.Application.ProjectManager.Generic;
using Tauron.Application.ProjectManager.UI;

namespace Tauron.Application.MgiProjectManager.LocalCache.Impl
{
    public abstract class CacheBase : ICacheBase
    {
        public virtual void SetServiceManager(ServiceManager manager)
        {
        }

        public abstract (IEnumerable<ICacheItem>, ICacheAction)? Fail(Delegate del, object[] parm);

        protected bool ValidateDelegate(Delegate del, string[] possibleMethods, out string name)
        {
            name = del.Method.Name;

            return possibleMethods.Contains(del.Method.Name);
        }

        [Serializable]
        public class CacheItemBase : ICacheItem
        {
            public CacheItemBase(Type identiferType, object content, string id, int order)
            {
                IdentiferType = identiferType;
                Content = content;
                Id = id;
                Order = order;
            }

            public Type IdentiferType { get; }
            public object Content { get; }
            public string Id { get; }
            public int Order { get; }
        }

        [Serializable]
        public class CacheActionBase : ICacheAction
        {
            private readonly string _method;
            private readonly Type _type;

            public CacheActionBase(Delegate del, string id, bool useSuffix)
            {
                Id = id;
                UseSuffix = useSuffix;
                _method = del.Method.Name;
                _type = del.Target.GetType();
                IdentiferType = del.Target.GetType().GetInterfaces().Single(i => i.Name != "ITypedClientHelperBase" && i.Name != "IClientHelperBase");
            }

            [PublicAPI] public string Suffix { get; set; } = "Direct";

            private bool UseSuffix { get; }

            public Type IdentiferType { get; }

            public string Id { get; }

            public void Try(List<ICacheItem> items, ClientObjectBase client)
            {
                var methodName = _method;

                if (UseSuffix)
                    methodName = _method + Suffix;

                var info = _type.GetMethod(methodName);
                if (info == null)
                {
                    LogManager.GetLogger("CacheAction-" + IdentiferType + Id).Log(LogLevel.Warn, $"Client Method Not Found -- {_method} -- {_type}");
                    return;
                }


                var length = info.GetParameters().Length;
                if (length == 0)
                    info.Invoke(client.CommunicationObject);
                else
                    info.Invoke(client.CommunicationObject,
                        items.Where(i => i.IdentiferType == IdentiferType && i.Id == Id).OrderBy(i => i.Order)
                            .Select(i => i.Content));
            }
        }
    }
}