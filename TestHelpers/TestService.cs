using System;

namespace TestHelpers
{
    public sealed class TestService<TTest> : IDisposable
    {
        private readonly ServicesConfiguration _configuration;
        public IServiceProvider ServiceProvider { get; }
        public TTest Service { get; }

        public TestService(ServicesConfiguration configuration, IServiceProvider serviceProvider, TTest service)
        {
            _configuration = configuration;
            ServiceProvider = serviceProvider;
            Service = service;
        }

        public void Assert()
        {
            foreach (var entry in _configuration.ServiceEntries) 
                entry.Assert();
        }

        public void Dispose() 
            => Assert();
    }
}