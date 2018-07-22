using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security;
using JetBrains.Annotations;
using NLog;
using Tauron.Application.MgiProjectManager.LocalCache.Impl;
using Tauron.Application.ProjectManager.Generic;
using Tauron.Application.ProjectManager.Services;
using Tauron.Application.ProjectManager.UI;

namespace Tauron.Application.MgiProjectManager.LocalCache
{
    public static class CacheManager
    {
        private static readonly Logger Logger = LogManager.GetLogger(nameof(CacheManager));

        private static readonly object Lock = new object();
        private static readonly WeakCollection<ServiceManager> RegistratedManagers = new WeakCollection<ServiceManager>();

        private static readonly Dictionary<Type, (Type CacheInterface, Type Impl)> CacheRegistry = new Dictionary<Type, (Type, Type)>
        { 
            {typeof(IJobManager), (typeof(IJobManagerCache), typeof(JobManagerCache))}
        };

        private static readonly BinaryFormatter BinaryFormatter = new BinaryFormatter();
        private static readonly string FileLocation = CommonApplication.Current.GetdefaultFileLocation().CombinePath("Cache.bin");
        private static readonly Dictionary<Type, Holder> Holders = new Dictionary<Type, Holder>();

        private static CacheCore _cacheCore;
        private static bool _readed;

        [PublicAPI]
        public static void SetServiceManager<TCache>(ServiceManager manager)
        {
            GetHolder(typeof(TCache)).ServiceManager = manager;
        }

        [PublicAPI]
        public static bool TakeCare<TCache, TParm>(Action<TParm> action, TParm parm)
        {
            return TakeCareCommon(typeof(TCache), action, new object[] {parm}, out _);
        }

        [PublicAPI]
        public static bool TakeCare<TCache, TResult, TParm>(Func<TParm, TResult> action, TParm parm, out TResult result)
            //where TResult : class
        {
            var val = TakeCareCommon(typeof(TCache), action, new object[] {parm}, out var temp);
            if (temp is TResult tmp)
                result = tmp;
            else
                result = default(TResult);

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

        private static bool TakeCareCommon(Type type, Delegate action, object[] parms, out object result)
        {
            TryRead();

            var cachePair = GetCache(type);

            var manager = cachePair.ServiceManager;

            if (manager == null || !manager.StatusOk || !manager.EnsureOpen(type))
            {
                Logger.Warn($"ServiceManager Status Not Ok: {cachePair.ServiceManager}");

                WriteFail(cachePair.Cache, action, parms);
                result = null;
                return false;
            }

            // ReSharper disable once PossibleNullReferenceException
            result = cachePair.ServiceManager.Secure(() => parms == null ? action.DynamicInvoke() : action.DynamicInvoke(parms), out var success);
            if (success) return true;

            WriteFail(cachePair.Cache, action, parms);
            return false;
        }

        private static void WriteFail(ICacheBase cacheBase, Delegate del, object[] parm)
        {
            var fail = cacheBase.Fail(del, parm);

            if (fail == null) return;

            _cacheCore.Actions.Add(fail.Value.Item2);
            foreach (var cacheItem in fail.Value.Item1)
                _cacheCore.CacheItems.Add(cacheItem);

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
                    _readed = true;
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

        private static (ICacheBase Cache, ServiceManager ServiceManager) GetCache(Type cacheType)
        {
            var holder = GetHolder(cacheType);
            if (holder.ServiceManager == null) throw new ArgumentNullException(nameof(cacheType), @"Service Manager Is Null");

            if (holder.Cache == null)
            {
                if (!CacheRegistry.TryGetValue(cacheType, out var real))
                {
                    Logger.Warn($"No CacheType Found for: {cacheType}");
                    throw new ArgumentNullException(nameof(cacheType), @"No Registrated Cache Type");
                }

                var cbase = Activator.CreateInstance(real.Impl) as ICacheBase;
                cbase?.SetServiceManager(holder.ServiceManager);
                holder.Cache = cbase;
            }

            var cacheBase = holder.Cache;
            var serviceManager = holder.ServiceManager;

            return (cacheBase, serviceManager);
        }

        private static Holder GetHolder(Type type)
        {
            if (Holders.TryGetValue(type, out var content)) return content;

            var newHolder = new Holder {ServiceManager = CommonApplication.Current.Container.Resolve<ServiceManager>()};

            Holders[type] = newHolder;

            return newHolder;
        }

        private static void RegisterServiceManager(ServiceManager manager)
        {
            lock (RegistratedManagers)
            {
                if (RegistratedManagers.Contains(manager)) return;

                RegistratedManagers.Add(manager);

                manager.ConnectionEstablished += ManagerOnConnectionEstablished;
            }
        }

        private static void ManagerOnConnectionEstablished(ServiceManager serviceManager, Type service, ClientObjectBase clientObject)
        {
            lock (Lock)
            {
                var actions = new List<ICacheAction>();
                var cacheItems = new List<ICacheItem>();

                foreach (var cacheAction in _cacheCore.Actions.Where(ca => ca.IdentiferType == service))
                {
                    try
                    {
                        cacheAction.Try(_cacheCore.CacheItems, clientObject);
                    }
                    catch (ClientException e)
                    {
                        Logger.Error(e);
                        continue;
                    }

                    actions.Add(cacheAction);
                    cacheItems.AddRange(_cacheCore.CacheItems.Where(i => i.Id == cacheAction.Id));
                }

                foreach (var cacheAction in actions)
                    _cacheCore.Actions.Remove(cacheAction);
                foreach (var cacheItem in cacheItems)
                    _cacheCore.CacheItems.Remove(cacheItem);
            }
        }

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
                    RegisterServiceManager(value);
                }
            }
        }
    }
}