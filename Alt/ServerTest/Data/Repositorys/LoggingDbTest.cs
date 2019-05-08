using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Serilog.Events;
using ServerTest.TestHelper;
using Tauron.MgiProjectManager.Data.Contexts;
using Tauron.MgiProjectManager.Data.Models;
using Tauron.MgiProjectManager.Data.Repositorys;
using Xunit;
using Xunit.Abstractions;

namespace ServerTest.Data.Repositorys
{
    public class LoggingDbTest : TestClassBase<LoggingDb>
    {
        public LoggingDbTest(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
            string name = Guid.NewGuid().ToString();
            LoggingDbContext.ConnectionBuilder = builder => builder.UseInMemoryDatabase(name);
        }

        protected override TestingObject<LoggingDb> GetTestingObject() 
            => base.GetTestingObject().AddContextDependecy(_ => new LoggingDbContext());

        private async Task FillDatabase()
        {
            using var db = new LoggingDbContext();

            await db.Events.AddRangeAsync(
                new LoggingEventEntity(new LogEvent(DateTimeOffset.Now, LogEventLevel.Warning, null, MessageTemplate.Empty, Array.Empty<LogEventProperty>())),
                new LoggingEventEntity(new LogEvent(DateTimeOffset.Now, LogEventLevel.Warning, null, MessageTemplate.Empty, Array.Empty<LogEventProperty>())));

            await db.SaveChangesAsync();
        }

        [Fact]
        public async Task LimitCount_Test()
        {
            var testObject = GetTestingObject();
            var db = testObject.GetResolvedTestingObject();
            var context = testObject.GetDependency<LoggingDbContext>();

            await FillDatabase();

            await db.LimitCount(1);

            Assert.Single(context.Events);
        } 
    }
}