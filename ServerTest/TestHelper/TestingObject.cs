using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace ServerTest.TestHelper
{
    public class TestingObject<T> where T : class
    {
        private Dictionary<Type, object> _dependencyMap { get; } = new Dictionary<Type, object>();

        public void AddDependency<TDependency>(TDependency dependency)
        {
            _dependencyMap.Add(typeof(TDependency), dependency);
        }

        public TDependency GetDependency<TDependency>() where TDependency : class
        {
            Type type = typeof(TDependency);

            if (!_dependencyMap.TryGetValue(type, out var dependency))
            {
                throw new Exception($"Testing object doesn't contain dependency of type {type}.");
            }

            return dependency as TDependency;
        }

        public void AddContextDependecy<TContext>(Func<DbContextOptions, TContext> factory)
        where TContext : DbContext
        {
            var ops = new DbContextOptionsBuilder<TContext>().UseInMemoryDatabase(typeof(TContext).Name).Options;

            AddDependency(factory(ops));
        }

        public T GetResolvedTestingObject()
        {
            IServiceCollection serviceCollection = new ServiceCollection();

            foreach (var dependency in _dependencyMap)
            {
                TypeInfo typeInfo = dependency.Key.GetTypeInfo();

                if (typeInfo.IsGenericType && typeInfo.GetGenericTypeDefinition() == typeof(Mock<>))
                {
                    PropertyInfo propertyInfo = dependency.Key.GetProperty("Object",
                        BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
                    object value = propertyInfo.GetValue(dependency.Value);

                    serviceCollection.AddSingleton(dependency.Key.GenericTypeArguments[0], value);
                }
                else
                {
                    serviceCollection.AddSingleton(dependency.Key, dependency.Value);
                }
            }

            IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();
            return ActivatorUtilities.CreateInstance<T>(serviceProvider);
        }
    }

}