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
using IObjectAdapter = Catel.Runtime.Serialization.IObjectAdapter;

namespace Tauron.Application.Wpf
{
    internal static class IocReplacer
    {
        private static MicrosoftServiceLocator? Locator;
        private static MicrosoftTypeFactory? _typeFactory;

        internal static void Create(IServiceCollection config)
        {
            Locator = new MicrosoftServiceLocator();
            _typeFactory = new MicrosoftTypeFactory(Locator);

            IoCFactory.CreateServiceLocatorFunc = () => Locator;
            IoCFactory.CreateTypeFactoryFunc = _ => _typeFactory;
            IoCFactory.CreateDependencyResolverFunc = _ => new CatelDependencyResolver(Locator);

            var locator = Locator;
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
            if (Locator != null)
            {
                Locator.InternalServiceProvider = provider;
                Locator.IsCreated = true;
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
            private 

            private readonly Dictionary<Type, RegistrationInfo> _registrationInfos = new Dictionary<Type, RegistrationInfo>();

            public IServiceCollection Collection { get; set; } = new ServiceCollection();

            public bool IsCreated { get; set; }

            public IServiceProvider InternalServiceProvider { get; set; } = new ServiceCollection().BuildServiceProvider();

            public object? GetService(Type serviceType)
            {
                object? result;

                try
                {
                    result = InternalServiceProvider.GetService(serviceType);


                    if (result == null && _registrationInfos.TryGetValue(serviceType, out var reg))
                    {
                        
                    }
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

            public RegistrationInfo? GetRegistrationInfo(Type serviceType, object? tag = null)
            {
                var type = Collection.FirstOrDefault(s => s.ServiceType == serviceType);

                if (type == null) return null;

                return Activator.CreateInstance(typeof(RegistrationInfo), BindingFlags.NonPublic, null,
                    new object?[]
                    {
                        type.ServiceType,
                        type.ImplementationType,
                        type.Lifetime == ServiceLifetime.Transient ? RegistrationType.Transient : RegistrationType.Singleton,
                        type.ImplementationInstance != null
                    }) as RegistrationInfo;
            }

            public bool IsTypeRegistered(Type serviceType, object? tag = null)
            {
                return Collection.Any(sr => sr.ServiceType == serviceType);
            }

            public bool IsTypeRegisteredAsSingleton(Type serviceType, object? tag = null)
            {
                return Collection.Any(sr => sr.ServiceType == serviceType && sr.Lifetime != ServiceLifetime.Transient) ||
                    _catelLocator.IsTypeRegisteredAsSingleton(serviceType, tag);
            }

            public bool IsTypeRegisteredWithOrWithoutTag(Type serviceType) => IsTypeRegistered(serviceType) || _catelLocator.IsTypeRegisteredWithOrWithoutTag(serviceType);

            public void RegisterInstance(Type serviceType, object instance, object? tag = null)
            {
                if(IsCreated)
                    _catelLocator.RegisterInstance(serviceType, instance, tag);
                else
                {
                    Collection.Add(new ServiceDescriptor(serviceType, instance));
                    TypeRegistered?.Invoke(this, new TypeRegisteredEventArgs(serviceType, instance.GetType(), null, RegistrationType.Singleton));
                }
            }

            public void RegisterType(Type serviceType, Type serviceImplementationType, object? tag = null, RegistrationType registrationType = RegistrationType.Singleton, bool registerIfAlreadyRegistered = true)
            {
                if(IsCreated)
                    _catelLocator.RegisterType(serviceType, serviceImplementationType, tag, registrationType, registerIfAlreadyRegistered);
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

            public void RegisterType(Type serviceType, Func<ServiceLocatorRegistration, object> createServiceFunc, object? tag = null, RegistrationType registrationType = RegistrationType.Singleton, bool registerIfAlreadyRegistered = true)
            {
                if(IsCreated)
                    _catelLocator.RegisterType(serviceType, createServiceFunc, tag, registrationType, registerIfAlreadyRegistered);
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

            public object? ResolveType(Type serviceType, object? tag = null) => GetService(serviceType);

            public IEnumerable<object> ResolveTypes(Type serviceType) 
                => InternalServiceProvider.GetServices(serviceType).Concat(_catelLocator.ResolveTypes(serviceType));

            public bool AreAllTypesRegistered(params Type[] types) 
                => AreMultipleTypesRegistered(types);

            public bool AreMultipleTypesRegistered(params Type[] types) =>
                types.All(t => Collection.Any(s => s.ServiceType == t)) ||
                _catelLocator.AreMultipleTypesRegistered(types);

            public object[] ResolveAllTypes(params Type[] types) 
                => types.Select(s => InternalServiceProvider.GetService(s) ?? _catelLocator.GetRequiredService(s)).ToArray();

            public object?[] ResolveMultipleTypes(params Type[] types) => types.Select(GetService).ToArray();

            public void RemoveType(Type serviceType, object? tag = null)
            {
                if (IsCreated)
                    _catelLocator.RemoveType(serviceType, tag);
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

            public void RemoveAllTypes(Type serviceType)
            {
                if(IsCreated)
                    _catelLocator.RemoveAllTypes(serviceType);
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

            public bool CanResolveNonAbstractTypesWithoutRegistration
            {
                get => _catelLocator.CanResolveNonAbstractTypesWithoutRegistration;
                set => _catelLocator.CanResolveNonAbstractTypesWithoutRegistration = value;
            }

            public bool AutoRegisterTypesViaAttributes { get; set; } = true;

            public bool IgnoreRuntimeIncorrectUsageOfRegisterAttribute { get; set; }

            public event EventHandler<MissingTypeEventArgs>? MissingType;

            public event EventHandler<TypeRegisteredEventArgs>? TypeRegistered;

            public event EventHandler<TypeUnregisteredEventArgs>? TypeUnregistered;

            public event EventHandler<TypeInstantiatedEventArgs>? TypeInstantiated;

            public void Dispose() 
                => _catelLocator.Dispose();
        }
    }
}