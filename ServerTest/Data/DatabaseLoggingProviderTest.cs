
using ServerTest.TestHelper;
using Tauron.MgiProjectManager.Data.Logging;
using Xunit.Abstractions;

namespace ServerTest.Data
{
    public class DatabaseLoggingProviderTest : TestBaseClass
    {
        public DatabaseLoggingProviderTest(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

        private TestingObject<DatabaseLoggingProvider> GetTestingObject()
        {
            var obj = new TestingObject<DatabaseLoggingProvider>()
                .AddLogger(TestOutputHelper);
        }
    }
}