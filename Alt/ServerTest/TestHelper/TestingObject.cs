﻿using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit.Abstractions;

namespace ServerTest.TestHelper
{
    public class TestingObject<T> : IServiceProvider where T : class
    {
        private class OptionsHolder<TType> : IOptions<TType> where TType : class, new()
        {
            public TType Value { get; }

            public OptionsHolder(TType value) => Value = value;
        }

        private Dictionary<Type, object> DependencyMap { get; } = new Dictionary<Type, object>();

        private IServiceProvider _serviceProvider;


        public TestingObject<T> AddDependency<TDependency>(TDependency dependency)
        {
            DependencyMap.Add(typeof(TDependency), dependency);
            return this;
        }

        public TestingObject<T> AddDependencyWithProvider<TDependency>(Func<IServiceProvider, TDependency> dependency)
        {
            DependencyMap.Add(typeof(TDependency), dependency(this));
            return this;
        }

        public TDependency GetDependency<TDependency>() where TDependency : class
        {
            Type type = typeof(TDependency);

            if (!DependencyMap.TryGetValue(type, out var dependency))
            {
                throw new Exception($"Testing object doesn't contain dependency of type {type}.");
            }

            return dependency as TDependency;
        }

        public TestingObject<T> BuildMock<TType>(params Action<Mock<TType>>[] config) where TType : class
        {
            var mock = new Mock<TType>();

            foreach (var action in config)
                action(mock);
            return AddDependency(mock);
        }

        public TestingObject<T> AddLogger(ITestOutputHelper testOutputHelper)
            => AddDependency<ILogger<T>>(testOutputHelper.BuildLoggerFor<T>());

        public TestingObject<T> AddOption<TType>(TType option) where TType : class, new() 
            => AddDependency<IOptions<TType>>(new OptionsHolder<TType>(option));

        public TestingObject<T> AddContextDependecy<TContext>(Func<DbContextOptions, TContext> factory)
        where TContext : DbContext
        {
            var ops = new DbContextOptionsBuilder<TContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

            return AddDependency(factory(ops));
        }

        public T GetResolvedTestingObject()
        {
            IServiceCollection serviceCollection = new ServiceCollection();

            foreach (var dependency in DependencyMap)
            {
                TypeInfo typeInfo = dependency.Key.GetTypeInfo();

                if (typeInfo.IsGenericType)
                {
                    var definition = typeInfo.GetGenericTypeDefinition();
                    if (definition == typeof(Mock<>))
                    {
                        PropertyInfo propertyInfo = dependency.Key.GetProperty("Object",
                                                                               BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
                        object value = propertyInfo.GetValue(dependency.Value);

                        serviceCollection.AddSingleton(dependency.Key.GenericTypeArguments[0], value);
                    }
                    else if (definition == typeof(Func<,>))
                        serviceCollection.AddTransient(typeInfo.GenericTypeParameters[1], (Func<IServiceProvider, object>) dependency.Value);
                    else
                        serviceCollection.AddSingleton(dependency.Key, dependency.Value);
                }
                else
                    serviceCollection.AddSingleton(dependency.Key, dependency.Value);
            }

            _serviceProvider = serviceCollection.BuildServiceProvider();
            return ActivatorUtilities.CreateInstance<T>(_serviceProvider);
        }

        public object GetService(Type serviceType) 
            => _serviceProvider?.GetService(serviceType);
    }

}