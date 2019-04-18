using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ServerTest.TestHelper;
using Tauron.MgiProjectManager;
using Tauron.MgiProjectManager.Data.Contexts;
using Tauron.MgiProjectManager.Data.Logging;
using Xunit;
using Xunit.Abstractions;

namespace ServerTest.Data
{
    public class DatabaseLoggingProviderTest : TestBaseClass<DatabaseLoggingProvider>
    {
        private string _name;

        public DatabaseLoggingProviderTest(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
            _name = Guid.NewGuid().ToString();
            LoggingDbContext.ConnectionBuilder = builder => builder.UseInMemoryDatabase(_name);
        }

        protected override TestingObject<DatabaseLoggingProvider> GetTestingObject() =>
            new TestingObject<DatabaseLoggingProvider>()
                .AddLogger(TestOutputHelper)
                .BuildMock<ILoggerFactory>(
                    m => m.Setup(lf => lf.CreateLogger("DatabaseLoggingProvider"))
                        // ReSharper disable once ExplicitCallerInfoArgument
                        .Returns(TestOutputHelper.BuildLogger("DatabaseLoggingProvider")))
                .AddOption(new AppSettings
                {
                    Logging = new LoggingConfig { BatchEntries = 2 }
                });

        [Fact(DisplayName = "Autostart Test")]
        public void Emit_Autostart_Test()
        {
            var testObject 
        }
    }
}