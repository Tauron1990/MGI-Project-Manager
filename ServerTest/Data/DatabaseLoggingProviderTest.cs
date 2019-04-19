using System;
using System.Linq;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Serilog.Events;
using ServerTest.TestHelper;
using Tauron.MgiProjectManager;
using Tauron.MgiProjectManager.Data.Contexts;
using Tauron.MgiProjectManager.Data.Logging;
using Xunit;
using Xunit.Abstractions;

namespace ServerTest.Data
{
    public class DatabaseLoggingProviderTest : TestClassBase<DatabaseLoggingProvider>
    {
        public DatabaseLoggingProviderTest(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
            var name = Guid.NewGuid().ToString();
            LoggingDbContext.ConnectionBuilder = builder => builder.UseInMemoryDatabase(name);
        }

        protected override TestingObject<DatabaseLoggingProvider> GetTestingObject() =>
            new TestingObject<DatabaseLoggingProvider>()
                .AddLogger(TestOutputHelper)
                .BuildMock<ILoggerFactory>(
                    m => m.Setup(lf => lf.CreateLogger("DatabaseLoggingProvider")).Verifiable(),
                        // ReSharper disable once ExplicitCallerInfoArgument
                        //.Returns(TestOutputHelper.BuildLogger("DatabaseLoggingProvider")).Verifiable(),
                    m => m.Verify(fac => fac.CreateLogger("DatabaseLoggingProvider"), Times.Never))
                .AddOption(new AppSettings
                {
                    Logging = new LoggingConfig { BatchEntries = 2 }
                });

        [Fact(DisplayName = "Autostart Test")]
        public void Emit_Autostart_Test()
        {
            var testObject = GetTestingObject();
            var provider = testObject.GetResolvedTestingObject();

            provider.Emit(new LogEvent(DateTimeOffset.Now, LogEventLevel.Warning, null, MessageTemplate.Empty, Array.Empty<LogEventProperty>()));
            provider.Emit(new LogEvent(DateTimeOffset.Now, LogEventLevel.Warning, null, MessageTemplate.Empty, Array.Empty<LogEventProperty>()));

            Thread.Sleep(TimeSpan.FromMilliseconds(5000));

            using var context = new LoggingDbContext();

            Assert.Equal(2, context.Events.Count());
        }

        [Fact(DisplayName = "Testing Dispose")]
        public void Emit_Dispose_Test()
        {
            var testObject = GetTestingObject();

            using (var provider = testObject.GetResolvedTestingObject())
                provider.Emit(new LogEvent(DateTimeOffset.Now, LogEventLevel.Warning, null, MessageTemplate.Empty, Array.Empty<LogEventProperty>()));

            using var context = new LoggingDbContext();

            Assert.Equal(1, context.Events.Count());
        }
    }
}