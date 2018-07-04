using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security;
using JetBrains.Annotations;
using NLog;
using Tauron.Application.MgiProjectManager.LocalCache.Impl;
using Tauron.Application.ProjectManager.Generic;

namespace Tauron.Application.MgiProjectManager.LocalCache
{
    public static class CacheManager
    {
        private class Holder
        {
            private ServiceManager _serviceManager;
            public ICacheBase Cache { get; set; }

            public ServiceManager ServiceManager
            {
                get => _serviceManager;
                set
                {
                    _serviceManager = value;
                    Cache?.SetServiceManager(_serviceManager);
                }
            }
        }

        private static readonly Logger Logger = LogManager.GetLogger(nameof(CacheManager));
 
        private static readonly object Lock = new object();
        private static readonly Dictionary<Type, Type> CacheRegistry = new Dictionary<Type, Type>
                                                                        {
                                                                            {typeof(IJobManagerCache), typeof(JobManagerCache)}
                                                                        };

        private static readonly BinaryFormatter BinaryFormatter = new BinaryFormatter();
        private static readonly string          FileLocation    = CommonApplication.Current.GetdefaultFileLocation().CombinePath("Cache.bin");
        private static readonly Dictionary<Type, Holder> Holders = new Dictionary<Type, Holder>();

        private static CacheCore _cacheCore;
        private static bool      _readed;

        [PublicAPI]
        public static void SetServiceManager<TCache>(ServiceManager manager) => GetHolder(typeof(TCache)).ServiceManager = manager;

        [PublicAPI]
        public static bool TakeCare<TCache, TParm>(Action<TParm> action, TParm parm) => TakeCareCommon(typeof(TCache), action, parm, out _);

        [PublicAPI]
        public static bool TakeCare<TCache, TResult, TParm>(Func<TParm, TResult> action, TParm parm,out TResult result)
            where TResult : class
        {
            var val = TakeCareCommon(typeof(TCache), action, parm, out var temp);
            result = temp as TResult;
            return val;
        }

        [PublicAPI]
        public static bool TakeCare<TCache, TResult>(Func<TResult> action, out TResult result)
            where TResult : class
        {
            var val = TakeCareCommon(typeof(TCache), action, null, out var temp);
            result = temp as TResult;
            return val;
        }

        private static bool TakeCareCommon(Type type, Delegate action, object parm, out object result)
        {
            TryRead();

            var holder = GetHolder(type);
            if (holder.ServiceManager == null) throw new ArgumentNullException(nameof(type), "Service Manager Is Null");


            if (holder.Cache == null)
            {
                if (CacheRegistry.TryGetValue(type, out var realType))
                    holder.Cache = (ICacheBase) Activator.CreateInstance(realType);
                else
                {
                    Logger.Warn($"No CacheType Found for: {type}");
                    throw new ArgumentNullException(nameof(type), "No Registrated Cache Type");
                }

                holder.Cache.SetServiceManager(holder.ServiceManager);
            }
            
            if (!holder.ServiceManager?.StatusOk ?? false)
            {
                Logger.Warn($"ServiceManager Status Not Ok: {holder.ServiceManager}");

                WriteFail(holder.Cache, action, parm);
                result = null;
                return false;
            }

            result = holder.ServiceManager.Secure(() => parm == null ? action.DynamicInvoke() : action.DynamicInvoke(parm), out var success);
            if (success) return true;

            WriteFail(holder.Cache, action, parm);
            return false;
        }

        private static void WriteFail(ICacheBase cacheBase, Delegate del, object parm)
        {
            var fail = cacheBase.Fail(del, parm);

            _cacheCore.Actions.Add(fail.Item2);
            _cacheCore.CacheItems.Add(fail.Item1);

            TryWrite();
        }

        private static void TryRead()
        {
            if (_readed)
                return;

            lock (Lock)
            {
                if (_readed) return;

                if (!FileLocation.ExisFile())
                {
                    _cacheCore = new CacheCore();
                    _readed    = true;
                    return;
                }

                try
                {
                    _cacheCore = FileLocation.Deserialize<CacheCore>(BinaryFormatter);
                }
                catch (Exception e) when (e is IOException || e is SecurityException || e is SerializationException)
                {
                }

                _readed = true;
            }
        }

        private static void TryWrite()
        {
            lock (Lock)
            {
                try
                {
                    _cacheCore.Serialize(BinaryFormatter, FileLocation);
                }
                catch (Exception e) when (e is IOException || e is SecurityException || e is SerializationException)
                {
                }
            }
        }

        private static ICacheBase Create(Type cacheType)
        {
            var holder = GetHolder(cacheType);
            if (holder.Cache != null) return holder.Cache;

            if (!CacheRegistry.TryGetValue(cacheType, out var real))
                return null;

            var cbase = Activator.CreateInstance(real) as ICacheBase;
            cbase?.SetServiceManager(holder.ServiceManager);
            holder.Cache = cbase;

            return cbase;
        }

        private static Holder GetHolder(Type type)
        {
            if (Holders.TryGetValue(type, out var content)) return content;

            var newHolder = new Holder {ServiceManager = CommonApplication.Current.Container.Resolve<ServiceManager>()};

            Holders[type] = newHolder;

            return newHolder;
        }
    }
}