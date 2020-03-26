using System;
using System.Threading.Tasks;

namespace TestHelpers
{
    public sealed class TestService<TTest>
    {
        private readonly ServicesConfiguration _configuration;

        public TestService(ServicesConfiguration configuration, IServiceProvider serviceProvider, TTest service)
        {
            _configuration = configuration;
            ServiceProvider = serviceProvider;
            Service = service;
        }

        public IServiceProvider ServiceProvider { get; }
        private TTest Service { get; }

        public void Test(Action<TTest> run)
        {
            run(Service);
            Assert();
        }

        public async Task Test(Func<TTest, Task> run)
        {
            await run(Service);
            Assert();
        }

        private void Assert()
        {
            foreach (var entry in _configuration.ServiceEntries)
                entry.Assert();
        }
    }
}