using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using Catel;
using Catel.Data;
using Catel.IoC;
using Catel.MVVM;
using Catel.MVVM.Auditing;
using Catel.MVVM.Views;
using Catel.Runtime.Serialization;
using Catel.Runtime.Serialization.Json;
using Catel.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Scrutor;

namespace Tauron.Application.Wpf
{
    internal static class IocReplacer
    {
        private static MicrosoftServiceLocator? _locator;
        private static MicrosoftTypeFactory? _typeFactory;

        internal static void Create(IServiceCollection config)
        {
            _locator = new MicrosoftServiceLocator();
            _typeFactory = new MicrosoftTypeFactory(_locator);

            IoCFactory.CreateServiceLocatorFunc = () => _locator;
            IoCFactory.CreateTypeFactoryFunc = _ => _typeFactory;
            IoCFactory.CreateDependencyResolverFunc = _ => new CatelDependencyResolver(_locator);

            var locator = _locator;
            locator.Collection = config;

            locator.Collection.Scan(
                ts =>
                    ts.FromApplicationDependencies()
                       .AddClasses(c => c.WithAttribute<ControlAttribute>())
                       .As<FrameworkElement>().UsingRegistrationStrategy(new ControlRegistrar())
                       .AddClasses().UsingAttributes());

            locator.Collection.AddTauronCommon();

            IoCConfiguration.UpdateDefaultComponents();
            var serviceLocator = IoCConfiguration.DefaultServiceLocator;
            serviceLocator.RegisterInstance(typeof(ITypeFactory), _typeFactory);

            locator.InternalServiceProvider = config.BuildServiceProvider();
            new CoreModule().Initialize(serviceLocator);

            //MVVMModule InCompatible
            //new MVVMModule().Initialize(serviceLocator);
            config.TryAddSingleton<Catel.Data.IObjectAdapter, ExpressionTreeObjectAdapter>();
            config.TryAddSingleton<IDataContextSubscriptionService, DataContextSubscriptionService>();
            config.TryAddSingleton<ICommandManager, CommandManager>();
            config.TryAddSingleton<IViewLoadManager, ViewLoadManager>();
            config.TryAddSingleton<IViewModelWrapperService, ViewModelWrapperService>();
            config.TryAddSingleton<IViewManager, ViewManager>();
            config.TryAddSingleton<IViewModelManager, ViewModelManager>();
            config.TryAddSingleton<IAutoCompletionService, AutoCompletionService>();
            config.TryAddSingleton<IWrapControlService, WrapControlService>();

            ViewModelServiceHelper.RegisterDefaultViewModelServices(serviceLocator);

            locator.Collection.AddSingleton<ISerializer, JsonSerializer>();
        }

        internal static void SetServiceProvider(IServiceProvider provider)
        {
            if (_locator != null)
            {
                _locator.InternalServiceProvider = provider;
                _locator.IsCreated = true;
            }
            else
                throw new InvalidOperationException();

            ITypeFactory typeFactory = provider.GetRequiredService<ITypeFactory>();
            AuditingManager.RegisterAuditor(typeFactory.CreateInstance<InvalidateCommandManagerOnViewModelInitializationAuditor>());
            AuditingManager.RegisterAuditor(typeFactory.CreateInstance<SubscribeKeyboardEventsOnViewModelCreationAuditor>());
            DesignTimeHelper.InitializeDesignTime();
        }

        private class MicrosoftTypeFactory : ITypeFactory
        {
            private readonly ConcurrentDictionary<Key, ObjectFactory> _objectFactories = new ConcurrentDictionary<Key, ObjectFactory>();

            private readonly IServiceProvider _provider;

            public MicrosoftTypeFactory(IServiceProvider provider) => _provider = provider;

            public object CreateInstance(Type typeToConstruct) => GetObjectFactory(typeToConstruct, Enumerable.Empty<object>())(_provider, new object[0]);

            public object CreateInstanceWithTag(Type typeToConstruct, object tag) => CreateInstance(typeToConstruct);

            public object CreateInstanceWithParameters(Type typeToConstruct, params object[] parameters) => GetObjectFactory(typeToConstruct, parameters)(_provider, parameters);

            public object CreateInstanceWithParametersWithTag(Type typeToConstruct, object tag, params object[] parameters) => CreateInstanceWithParameters(typeToConstruct, parameters);

            public object CreateInstanceWithParametersAndAutoCompletion(Type typeToConstruct, params object[] parameters) => CreateInstanceWithParameters(typeToConstruct, parameters);

            public object CreateInstanceWithParametersAndAutoCompletionWithTag(Type typeToConstruct, object tag, params object[] parameters) => CreateInstanceWithParameters(typeToConstruct, parameters);

            public void ClearCache() => _objectFactories.Clear();

            private ObjectFactory GetObjectFactory(Type forType, IEnumerable<object> args)
            {
                return _objectFactories.GetOrAdd(
                    new Key(forType, args.Select(o => o.GetType()).ToArray()),
                    type => ActivatorUtilities.CreateFactory(type.Type, type.Arguments));
            }

            private sealed class Key : IEquatable<Key>
            {
                public readonly Type[] Arguments;
                public readonly Type Type;

                public Key(Type type, Type[] arguments)
                {
                    Type = type;
                    Arguments = arguments;
                }

                public bool Equals(Key? other)
                {
                    if (ReferenceEquals(null, other)) return false;
                    if (ReferenceEquals(this, other)) return true;
                    return Type == other.Type && Arguments.SequenceEqual(other.Arguments);
                }

                public override bool Equals(object? obj)
                {
                    if (ReferenceEquals(null, obj)) return false;
                    if (ReferenceEquals(this, obj)) return true;
                    return obj.GetType() == GetType() && Equals((Key) obj);
                }

                public override int GetHashCode()
                {
                    unchecked
                    {
                        var start = Type.GetHashCode() * 397;

                        return Arguments.Aggregate(start, (current, arg) => unchecked(current * 31 + arg.GetHashCode()));
                    }
                }

                public static bool operator ==(Key left, Key right) => Equals(left, right);

                public static bool operator !=(Key left, Key right) => !Equals(left, right);
            }
        }

        private class MicrosoftServiceLocator : IServiceLocator
        {
            private sealed class ServiceInfo
            {
                private readonly int _hash;

                public Type Type { get; }

                private object? Tag { get; }

                public ServiceInfo(Type type, object? tag)
                {
                    Type = type;
                    Tag = tag;


                    _hash = HashHelper.CombineHash(Type.GetHashCode(), Tag?.GetHashCode() ?? 0);
                }

                public override int GetHashCode() => _hash;

                public override bool Equals(object? obj)
                {
                    if (!(obj is ServiceInfo serviceInfo))
                        return false;
                    if (serviceInfo._hash != _hash)
                        return false;
                    return serviceInfo.Type == Type && Equals(serviceInfo.Tag, Tag);
                }
            }

            private class InternalRegistrationInfo : IDisposable
            {
                public ServiceLocatorRegistration LocatorRegistration { get; }

                private readonly bool _hasFactory;

                private object? _instance;

                private ObjectFactory? _factory;

                public InternalRegistrationInfo(bool hasFactory, ServiceLocatorRegistration locatorRegistration)
                {
                    _hasFactory = hasFactory;
                    LocatorRegistration = locatorRegistration;
                }

                public object Create(IServiceProvider provider)
                {
                    switch (LocatorRegistration.RegistrationType)
                    {
                        case RegistrationType.Singleton:
                            if (_instance != null)
                                return _instance;
                            if (_hasFactory)
                            {
                                _instance = LocatorRegistration.CreateServiceFunc(LocatorRegistration);
                                if (!(_instance is Type type)) return _instance;

                                _instance = ActivatorUtilities.CreateInstance(provider, type);
                                return _instance;
                            }
                            _instance = ActivatorUtilities.CreateInstance(provider, LocatorRegistration.ImplementingType);
                            return _instance;
                        case RegistrationType.Transient:
                            var targetType = LocatorRegistration.ImplementingType;

                            if (_hasFactory)
                            {
                                var temp = LocatorRegistration.CreateServiceFunc(LocatorRegistration);
                                if (temp is Type type)
                                    targetType = type;
                                else
                                    return temp;
                            }
                            _factory ??= ActivatorUtilities.CreateFactory(targetType, Type.EmptyTypes);
                            return _factory(provider, Array.Empty<object>());
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                public void Dispose()
                {
                    if(_instance is IDisposable disposable)
                        disposable.Dispose();
                }
            }
            
            private readonly object _locker = new object();
            private readonly Dictionary<ServiceInfo, InternalRegistrationInfo> _registrationInfos = new Dictionary<ServiceInfo, InternalRegistrationInfo>();

            public IServiceCollection Collection { get; set; } = new ServiceCollection();

            public bool IsCreated { get; set; }

            public IServiceProvider InternalServiceProvider { get; set; } = new ServiceCollection().BuildServiceProvider();

            private object? GetService(Type serviceType, object? tag)
            {
                lock (_locker)
                {
                    object? result;

                    try
                    {
                        result = InternalServiceProvider.GetService(serviceType);


                        if (result == null && _registrationInfos.TryGetValue(new ServiceInfo(serviceType, tag), out var reg))
                            return reg.Create(InternalServiceProvider);
                    }
                    catch (TypeNotRegisteredException)
                    {
                        result = null;
                    }

                    if (result != null) return result;

                    var eventArgs = new MissingTypeEventArgs(serviceType);
                    MissingType?.Invoke(this, eventArgs);
                    result = eventArgs.ImplementingInstance;

                    return result;
                }
            }

            public RegistrationInfo? GetRegistrationInfo(Type serviceType, object? tag = null)
            {
                lock (_locker)
                {
                    var type = Collection.FirstOrDefault(s => s.ServiceType == serviceType);

                    if (type != null)
                    {
                        return Activator.CreateInstance(typeof(RegistrationInfo), BindingFlags.NonPublic, null,
                            new object?[]
                            {
                                type.ServiceType,
                                type.ImplementationType,
                                type.Lifetime == ServiceLifetime.Transient ? RegistrationType.Transient : RegistrationType.Singleton,
                                type.ImplementationInstance != null
                            }) as RegistrationInfo;
                    }

                    var key = new ServiceInfo(serviceType, tag);
                    if (_registrationInfos.TryGetValue(key, out var reg))
                    {
                        return Activator.CreateInstance(typeof(RegistrationInfo), BindingFlags.NonPublic, null,
                            new object?[]
                            {
                                reg.LocatorRegistration.DeclaringType,
                                reg.LocatorRegistration.ImplementingType,
                                reg.LocatorRegistration.RegistrationType,
                                false
                            }) as RegistrationInfo;
                    }

                    return null;
                }
            }

            public bool IsTypeRegistered(Type serviceType, object? tag = null)
            {
                lock (_locker)
                {
                    return Collection.Any(sr => sr.ServiceType == serviceType) || 
                        _registrationInfos.ContainsKey(new ServiceInfo(serviceType, tag));
                }
            }

            public bool IsTypeRegisteredAsSingleton(Type serviceType, object? tag = null)
            {
                lock (_locker)
                {
                    return Collection.Any(sr => sr.ServiceType == serviceType && sr.Lifetime != ServiceLifetime.Transient) ||
                        (_registrationInfos.TryGetValue(new ServiceInfo(serviceType, tag), out var reg) && reg.LocatorRegistration.RegistrationType == RegistrationType.Singleton);
                }
            }

            public bool IsTypeRegisteredWithOrWithoutTag(Type serviceType)
            {
                lock (_locker)
                {
                    return _registrationInfos != null && (IsTypeRegistered(serviceType) ||
                        _registrationInfos.Any(si => si.Key.Type == serviceType));
                }
            }

            public void RegisterInstance(Type serviceType, object instance, object? tag = null)
            {
                lock (_locker)
                {
                    if(IsCreated)
                    {
                        _registrationInfos[new ServiceInfo(serviceType, tag)] = new InternalRegistrationInfo(true,
                            new ServiceLocatorRegistration(serviceType, serviceType, tag, RegistrationType.Singleton, r => instance));
                    }
                    else
                    {
                        Collection.Add(new ServiceDescriptor(serviceType, instance));
                        TypeRegistered?.Invoke(this, new TypeRegisteredEventArgs(serviceType, instance.GetType(), null, RegistrationType.Singleton));
                    }
                }
            }

            public void RegisterType(Type serviceType, Type serviceImplementationType, object? tag = null, RegistrationType registrationType = RegistrationType.Singleton, bool registerIfAlreadyRegistered = true)
            {
                lock (_locker)
                {
                    if (IsCreated)
                    {
                        if(IsTypeRegistered(serviceType, tag) && !registerIfAlreadyRegistered)
                            return;

                        _registrationInfos[new ServiceInfo(serviceType, tag)] = new InternalRegistrationInfo(false, 
                            new ServiceLocatorRegistration(serviceType, serviceImplementationType, tag, registrationType, d => null));
                    }
                    else
                    {
                        if (registerIfAlreadyRegistered)
                        {
                            Collection.Add(new ServiceDescriptor(serviceType, serviceImplementationType,
                                registrationType == RegistrationType.Singleton ? ServiceLifetime.Singleton : ServiceLifetime.Transient));
                        }
                        else
                        {
                            Collection.TryAdd(new ServiceDescriptor(serviceType, serviceImplementationType,
                                registrationType == RegistrationType.Singleton ? ServiceLifetime.Singleton : ServiceLifetime.Transient));
                        }

                        TypeRegistered?.Invoke(this, new TypeRegisteredEventArgs(serviceType, serviceImplementationType, tag, registrationType));
                    }
                }
            }

            public void RegisterType(Type serviceType, Func<ServiceLocatorRegistration, object> createServiceFunc, object? tag = null, RegistrationType registrationType = RegistrationType.Singleton, bool registerIfAlreadyRegistered = true)
            {
                lock (_locker)
                {
                    if (IsCreated)
                    {
                        if (IsTypeRegistered(serviceType, tag) && !registerIfAlreadyRegistered)
                            return;
                    
                        _registrationInfos[new ServiceInfo(serviceType, tag)] = new InternalRegistrationInfo(true, 
                            new ServiceLocatorRegistration(serviceType, serviceType, tag, registrationType, createServiceFunc));
                    }
                    else
                    {

                        var descriptor = new ServiceDescriptor(
                            serviceType,
                            p => createServiceFunc(new ServiceLocatorRegistration(serviceType, serviceType, tag ?? p, registrationType, createServiceFunc)),
                            registrationType == RegistrationType.Transient ? ServiceLifetime.Transient : ServiceLifetime.Singleton);

                        if (registerIfAlreadyRegistered)
                            Collection.Add(descriptor);
                        else
                            Collection.TryAdd(descriptor);

                        TypeRegistered?.Invoke(this, new TypeRegisteredEventArgs(serviceType, null, tag, registrationType));
                    }
                }
            }

            public object? ResolveType(Type serviceType, object? tag = null) => GetService(serviceType, tag);

            public IEnumerable<object> ResolveTypes(Type serviceType)
            {
                lock (_locker)
                {
                    return InternalServiceProvider.GetServices(serviceType).Concat(_registrationInfos.Values
                       .Where(r => r.LocatorRegistration.DeclaringType == serviceType)
                       .Select(r => r.Create(InternalServiceProvider)));
                }
            }

            public bool AreAllTypesRegistered(params Type[] types) 
                => AreMultipleTypesRegistered(types);

            public bool AreMultipleTypesRegistered(params Type[] types)
            {
                lock (_locker)
                {
                    return types.All(t => Collection.Any(s => s.ServiceType == t) || _registrationInfos.Keys.Any(r => r.Type == t));
                }
            }

            public object[] ResolveAllTypes(params Type[] types)
            {
                lock (_locker)
                {
                    IEnumerable<object> Create(Type t)
                    {
                        yield return InternalServiceProvider.GetService(t);

                        foreach (var reg in _registrationInfos.Where(reg => reg.Key.Type == t))
                            yield return reg.Value.Create(InternalServiceProvider);
                    }

                    return types.SelectMany(Create).Where(o => o != null).ToArray();
                }
            }

            public object?[] ResolveMultipleTypes(params Type[] types) 
                => types.Select(t => GetService(t)).ToArray();

            public void RemoveType(Type serviceType, object? tag = null)
            {
                lock (_locker)
                {
                    if (IsCreated)
                        _registrationInfos.Remove(new ServiceInfo(serviceType, tag));
                    else
                    {
                        var desc = Collection.FirstOrDefault(sd => sd.ServiceType == serviceType);
                        if (desc == null) return;

                        if (Collection.Remove(desc))
                        {
                            TypeUnregistered?.Invoke(this,
                                new TypeUnregisteredEventArgs(desc.ServiceType, desc.ImplementationType, null,
                                    desc.Lifetime == ServiceLifetime.Transient ? RegistrationType.Transient : RegistrationType.Singleton,
                                    desc.ImplementationInstance));
                        }

                        TypeUnregistered?.Invoke(this, new TypeUnregisteredEventArgs(desc.ServiceType, desc.ImplementationType, tag, desc.Lifetime == ServiceLifetime.Transient ? RegistrationType.Transient : RegistrationType.Singleton));
                    }
                }
            }

            public void RemoveAllTypes(Type serviceType)
            {
                lock (_locker)
                {
                    if (IsCreated)
                    {
                        foreach (var b in _registrationInfos.Keys.Where(k => k.Type == serviceType)) 
                            _registrationInfos.Remove(b);
                    }
                    else
                    {
                        var desc = Collection.FirstOrDefault(sd => sd.ServiceType == serviceType);
                        while (desc != null)
                        {
                            if (Collection.Remove(desc))
                            {
                                TypeUnregistered?.Invoke(this,
                                    new TypeUnregisteredEventArgs(desc.ServiceType, desc.ImplementationType, null,
                                        desc.Lifetime == ServiceLifetime.Transient ? RegistrationType.Transient : RegistrationType.Singleton,
                                        desc.ImplementationInstance));

                                TypeUnregistered?.Invoke(this, new TypeUnregisteredEventArgs(desc.ServiceType, desc.ImplementationType, null, desc.Lifetime == ServiceLifetime.Transient ? RegistrationType.Transient : RegistrationType.Singleton));

                                desc = Collection.FirstOrDefault(sd => sd.ServiceType == serviceType);
                            }
                            else
                                break;
                        }
                    }
                }
            }

            public bool CanResolveNonAbstractTypesWithoutRegistration
            {
                get => false;
                set{}
            }

            public bool AutoRegisterTypesViaAttributes { get; set; } = true;

            public bool IgnoreRuntimeIncorrectUsageOfRegisterAttribute { get; set; }

            public event EventHandler<MissingTypeEventArgs>? MissingType;

            public event EventHandler<TypeRegisteredEventArgs>? TypeRegistered;

            public event EventHandler<TypeUnregisteredEventArgs>? TypeUnregistered;

            public event EventHandler<TypeInstantiatedEventArgs>? TypeInstantiated;

            public void Dispose()
            {
                lock (_locker)
                {
                    foreach (var internalRegistrationInfo in _registrationInfos) 
                        internalRegistrationInfo.Value.Dispose();

                    _registrationInfos.Clear();
                }
            }

            public object GetService(Type serviceType) => GetService(serviceType, null) ?? throw new MissingTypeRegistrationException(serviceType);
        }
    }
}