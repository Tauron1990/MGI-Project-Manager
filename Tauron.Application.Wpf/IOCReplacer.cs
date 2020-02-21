using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using Catel;
using Catel.IoC;
using Catel.Runtime.Serialization;
using Catel.Runtime.Serialization.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Tauron.Application.Wpf
{
    public static class IOCReplacer
    {
        private static readonly Lazy<MicrosoftServiceLocator> Locator = new Lazy<MicrosoftServiceLocator>(() => new MicrosoftServiceLocator(new ServiceCollection()));
        private static readonly Lazy<MicrosoftTypeFactory> TypeFactory = new Lazy<MicrosoftTypeFactory>(() => new MicrosoftTypeFactory(Locator.Value));

        public static IServiceLocator Create(Action<ServiceCollection> config)
        {
            var locator = Locator.Value;

            locator.Collection.Scan(
                ts =>
                    ts.FromApplicationDependencies()
                        .AddClasses(c => c.WithAttribute<ControlAttribute>())
                        .As<FrameworkElement>().UsingRegistrationStrategy(new ControlRegistrar())
                        .AddClasses().UsingAttributes());

            config(locator.Collection);

            IoCFactory.CreateServiceLocatorFunc = () => Locator.Value;
            IoCFactory.CreateTypeFactoryFunc = _ => TypeFactory.Value;

            var serviceLocator = IoCFactory.CreateServiceLocatorFunc();

            if (!serviceLocator.IsTypeRegistered<IDependencyResolver>())
            {
                var dependencyResolver = IoCFactory.CreateDependencyResolverFunc(serviceLocator);
                serviceLocator.RegisterInstance(typeof(IDependencyResolver), dependencyResolver);
            }

            if (!serviceLocator.IsTypeRegistered<ITypeFactory>())
            {
                var typeFactory = IoCFactory.CreateTypeFactoryFunc(serviceLocator);
                serviceLocator.RegisterInstance(typeof(ITypeFactory), typeFactory);
            }

            new CoreModule().Initialize(serviceLocator);
            new MVVMModule().Initialize(serviceLocator);
            locator.Collection.AddSingleton<ISerializer, JsonSerializer>();

            return locator;
        }

        private class MicrosoftTypeFactory : ITypeFactory
        {
            private readonly ConcurrentDictionary<Key, ObjectFactory> _objectFactories = new ConcurrentDictionary<Key, ObjectFactory>();

            private readonly IServiceProvider _provider;

            public MicrosoftTypeFactory(IServiceProvider provider)
            {
                _provider = provider;
            }

            public object CreateInstance(Type typeToConstruct)
            {
                return GetObjectFactory(typeToConstruct, Enumerable.Empty<object>())(_provider, new object[0]);
            }

            public object CreateInstanceWithTag(Type typeToConstruct, object tag)
            {
                return CreateInstance(typeToConstruct);
            }

            public object CreateInstanceWithParameters(Type typeToConstruct, params object[] parameters)
            {
                return GetObjectFactory(typeToConstruct, parameters)(_provider, parameters);
            }

            public object CreateInstanceWithParametersWithTag(Type typeToConstruct, object tag, params object[] parameters)
            {
                return CreateInstanceWithParameters(typeToConstruct, parameters);
            }

            public object CreateInstanceWithParametersAndAutoCompletion(Type typeToConstruct, params object[] parameters)
            {
                return CreateInstanceWithParameters(typeToConstruct, parameters);
            }

            public object CreateInstanceWithParametersAndAutoCompletionWithTag(Type typeToConstruct, object tag, params object[] parameters)
            {
                return CreateInstanceWithParameters(typeToConstruct, parameters);
            }

            public void ClearCache()
            {
                _objectFactories.Clear();
            }

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

                public bool Equals(Key other)
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

                public static bool operator ==(Key left, Key right)
                {
                    return Equals(left, right);
                }

                public static bool operator !=(Key left, Key right)
                {
                    return !Equals(left, right);
                }
            }
        }

        private class MicrosoftServiceLocator : IServiceLocator
        {
            private ServiceProvider? _serviceProvider;

            public MicrosoftServiceLocator(ServiceCollection collection)
            {
                Collection = collection;
            }

            public ServiceCollection Collection { get; }

            private IServiceProvider InternalServiceProvider
                => _serviceProvider ??= Collection.BuildServiceProvider();

            public object? GetService(Type serviceType)
            {
                var result = InternalServiceProvider.GetService(serviceType);

                if (result != null) return result;

                var eventArgs = new MissingTypeEventArgs(serviceType);
                MissingType?.Invoke(this, eventArgs);
                result = eventArgs.ImplementingInstance;

                return result;
            }

            public void Dispose()
            {
                _serviceProvider?.Dispose();
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
                return Collection.Any(sr => sr.ServiceType == serviceType && sr.Lifetime != ServiceLifetime.Transient);
            }

            public bool IsTypeRegisteredWithOrWithoutTag(Type serviceType)
            {
                return IsTypeRegistered(serviceType);
            }

            public void RegisterInstance(Type serviceType, object instance, object? tag = null)
            {
                ValidateCollectionChange();

                Collection.Add(new ServiceDescriptor(serviceType, instance));
                TypeRegistered?.Invoke(this, new TypeRegisteredEventArgs(serviceType, instance.GetType(), null, RegistrationType.Singleton));
            }

            public void RegisterType(Type serviceType, Type serviceImplementationType, object? tag = null, RegistrationType registrationType = RegistrationType.Singleton, bool registerIfAlreadyRegistered = true)
            {
                ValidateCollectionChange();

                if (registerIfAlreadyRegistered)
                    Collection.Add(new ServiceDescriptor(serviceType, serviceImplementationType,
                        registrationType == RegistrationType.Singleton ? ServiceLifetime.Singleton : ServiceLifetime.Transient));
                else
                    Collection.TryAdd(new ServiceDescriptor(serviceType, serviceImplementationType,
                        registrationType == RegistrationType.Singleton ? ServiceLifetime.Singleton : ServiceLifetime.Transient));

                TypeRegistered?.Invoke(this, new TypeRegisteredEventArgs(serviceType, serviceImplementationType, tag, registrationType));
            }

            public void RegisterType(Type serviceType, Func<ServiceLocatorRegistration, object> createServiceFunc, object? tag = null, RegistrationType registrationType = RegistrationType.Singleton, bool registerIfAlreadyRegistered = true)
            {
                ValidateCollectionChange();

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

            public object? ResolveType(Type serviceType, object? tag = null)
            {
                return GetService(serviceType);
            }

            public IEnumerable<object> ResolveTypes(Type serviceType)
            {
                return InternalServiceProvider.GetServices(serviceType);
            }

            public bool AreAllTypesRegistered(params Type[] types)
            {
                return types.All(t => Collection.Any(s => s.ServiceType == t));
            }

            public bool AreMultipleTypesRegistered(params Type[] types)
            {
                return types.Any(t => Collection.Any(s => s.ServiceType == t));
            }

            public object[] ResolveAllTypes(params Type[] types)
            {
                return types.Select(s => InternalServiceProvider.GetRequiredService(s)).ToArray();
            }

            public object?[] ResolveMultipleTypes(params Type[] types)
            {
                return types.Select(GetService).ToArray();
            }

            public void RemoveType(Type serviceType, object? tag = null)
            {
                ValidateCollectionChange();
                var desc = Collection.FirstOrDefault(sd => sd.ServiceType == serviceType);
                if (desc == null) return;

                if (Collection.Remove(desc))
                    TypeUnregistered?.Invoke(this,
                        new TypeUnregisteredEventArgs(desc.ServiceType, desc.ImplementationType, null,
                            desc.Lifetime == ServiceLifetime.Transient ? RegistrationType.Transient : RegistrationType.Singleton,
                            desc.ImplementationInstance));
                TypeUnregistered?.Invoke(this, new TypeUnregisteredEventArgs(desc.ServiceType, desc.ImplementationType, tag, desc.Lifetime == ServiceLifetime.Transient ? RegistrationType.Transient : RegistrationType.Singleton));
            }

            public void RemoveAllTypes(Type serviceType)
            {
                ValidateCollectionChange();
                var desc = Collection.FirstOrDefault(sd => sd.ServiceType == serviceType);
                while (desc != null)
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
                    {
                        break;
                    }
            }

            public bool CanResolveNonAbstractTypesWithoutRegistration
            {
                get => false;
                set { }
            }

            public bool AutoRegisterTypesViaAttributes { get; set; } = true;

            public bool IgnoreRuntimeIncorrectUsageOfRegisterAttribute { get; set; }

            public event EventHandler<MissingTypeEventArgs>? MissingType;

            public event EventHandler<TypeRegisteredEventArgs>? TypeRegistered;

            public event EventHandler<TypeUnregisteredEventArgs>? TypeUnregistered;

            public event EventHandler<TypeInstantiatedEventArgs>? TypeInstantiated
            {
                add => throw new NotSupportedException();
                remove => throw new NotSupportedException();
            }

            private void ValidateCollectionChange()
            {
                if (_serviceProvider == null) return;

                throw new InvalidOperationException("Service Provider wurde schon erstellt");
            }
        }
    }
}