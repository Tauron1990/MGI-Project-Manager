using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Tauron.Application.Ioc;
using Tauron.Application.ProjectManager.ApplicationServer.BussinesLayer;
using Tauron.Application.ProjectManager.ApplicationServer.Data;
using Tauron.Application.ProjectManager.Services.Data.Entitys;
using Tauron.Application.ProjectManager.Services.DTO;
using TestHelperLib;
using Xunit;

namespace ApplicationServer.Tests.BussinesLayer
{
    public class TimeCalculationSaveRuleTest : IDisposable
    {
        private readonly IContainer _container;
        private readonly InMemoryDatabaseRoot _inMemoryDatabaseRoot = new InMemoryDatabaseRoot();

        public TimeCalculationSaveRuleTest()
        {
            SetupApplication.AddSetupAction(() => DatabaseImpl.UpdateSchema(o => o.UseInMemoryDatabase("test", _inMemoryDatabaseRoot)));
            SetupApplication.AddTypes(typeof(MainDatabase));
            _container = SetupApplication.Setup(true, typeof(AddSetupItemRule).Assembly);
        }

        public void Dispose() => SetupApplication.Free();

        [Fact]
        public void Test()
        {
            string testJob = "BM18_00001";

            using (var db = new DatabaseImpl())
            {
                db.Jobs.Add(new JobEntity
                {
                    Id = testJob,
                    LongName = testJob,
                    Status = JobStatus.InProgress,
                    TargetDate = DateTime.Now + TimeSpan.FromDays(10)
                });

                db.SaveChanges();
            }

            var rule = _container.GetIoBusinessRule<SaveInput, SaveOutput>(RuleNames.TimeCalculationSaving);
        }
    }
}