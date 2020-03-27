using System;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Tauron.Application.Logging;
using Xunit.Abstractions;

namespace TestHelpers
{
    public static class ServiceTest
    {
        public static TestService<TInterface> Create<TInterface, TTest>(ITestOutputHelper helper, Func<TTest>? factory = null, Action<ServicesConfiguration>? config = null)
            where TTest : class, TInterface where TInterface : class
        {
            var collection = new ServiceCollection();

            collection.AddOptions();

            if (factory != null)
                collection.AddSingleton<TInterface>(s => factory());
            else
                collection.AddSingleton<TInterface, TTest>();

            Log.Logger = new LoggerConfiguration().ConfigDefaultLogging("Test", noFile: true).WriteTo.TestOutput(helper).CreateLogger();
            collection.AddSingleton(Log.Logger);
            collection.AddTauronLogging();

            var configuration = new ServicesConfiguration(collection);
            config?.Invoke(configuration);
            foreach (var entry in configuration.ServiceEntries)
                entry.Register(collection);

            var provider = collection.BuildServiceProvider();
            return new TestService<TInterface>(configuration, provider, (TTest) provider.GetRequiredService<TInterface>());
        }

        public static TestService<TInterface> Create<TInterface>(ITestOutputHelper helper, Func<TInterface>? factory = null, Action<ServicesConfiguration>? config = null)
            where TInterface : class
        {
            var collection = new ServiceCollection();

            collection.AddOptions();

            if (factory != null)
                collection.AddSingleton(s => factory());
            else
                collection.AddSingleton<TInterface, TInterface>();

            Log.Logger = new LoggerConfiguration().ConfigDefaultLogging("Test", noFile: true).WriteTo.TestOutput(helper).CreateLogger();
            collection.AddSingleton(Log.Logger);
            collection.AddTauronLogging();

            var configuration = new ServicesConfiguration(collection);
            config?.Invoke(configuration);
            foreach (var entry in configuration.ServiceEntries)
                entry.Register(collection);

            var provider = collection.BuildServiceProvider();
            return new TestService<TInterface>(configuration, provider, provider.GetRequiredService<TInterface>());
        }
    }
}