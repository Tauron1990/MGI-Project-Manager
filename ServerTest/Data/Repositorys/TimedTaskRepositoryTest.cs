using System;
using System.Linq;
using System.Threading.Tasks;
using ServerTest.TestHelper;
using Tauron.MgiProjectManager.Data.Contexts;
using Tauron.MgiProjectManager.Data.Models;
using Tauron.MgiProjectManager.Data.Repositorys;
using Xunit;
using Xunit.Abstractions;

namespace ServerTest.Data.Repositorys
{
    public class TimedTaskRepositoryTest : TestClassBase<TimedTaskRepository>
    {
        public TimedTaskRepositoryTest(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        { }

        protected override TestingObject<TimedTaskRepository> GetTestingObject() 
            => base.GetTestingObject().AddContextDependecy(op => new ApplicationDbContext(op));

        [Fact]
        public async Task GetTasks_Test()
        {
            var testObject = GetTestingObject();
            var repo       = testObject.GetResolvedTestingObject();
            var context    = testObject.GetDependency<ApplicationDbContext>();

            await context.TimedTasks.AddRangeAsync(new TimedTaskEntity {LastRun = DateTime.Now, Name = "Temp"},
                                                   new TimedTaskEntity {LastRun = DateTime.Now, Name = "Temp2"});

            var tasks = await repo.GetTasks();

            Assert.Equal(2, tasks.Count());
        }

        [Fact]
        public async Task UpdateTime_Test()
        {
            var testObject = GetTestingObject();
            var repo       = testObject.GetResolvedTestingObject();
            var context    = testObject.GetDependency<ApplicationDbContext>();

            await context.TimedTasks.AddRangeAsync(new TimedTaskEntity { LastRun = new DateTime(2015, 1,1), Name = "Temp" },
                                                   new TimedTaskEntity { LastRun = new DateTime(2015,1,1), Name = "Temp2" });

            var dateNow = DateTime.Now;

            await repo.UpdateTime("Temp");

            foreach (var contextTimedTask in context.TimedTasks)
            {
                switch (contextTimedTask.Name)
                {
                    case "Temp":
                        Assert.Equal(dateNow.Year, contextTimedTask.LastRun.Year);
                        break;
                    case "Temp2":
                        Assert.Equal(2015, contextTimedTask.LastRun.Year);
                        break;
                    default:
                        throw new InvalidOperationException("Unkown Time Task");
                }
            }
        }
    }
}