using System;
using System.Linq;
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
    public class InsertJobRuleTest : IDisposable
    {
        private readonly IContainer _container;
        private readonly InMemoryDatabaseRoot _inMemoryDatabaseRoot = new InMemoryDatabaseRoot();

        public InsertJobRuleTest()
        {
            SetupApplication.AddSetupAction(() => DatabaseImpl.UpdateSchema(o => o.UseInMemoryDatabase("test", _inMemoryDatabaseRoot)));
            SetupApplication.AddTypes(typeof(MainDatabase));
            _container = SetupApplication.Setup(true, typeof(AddSetupItemRule).Assembly);
        }

        public void Dispose() => SetupApplication.Free();

        [Fact]
        public void TestMethod()
        {
            var rule = _container.GetIoBusinessRule<JobItemDto, bool>(RuleNames.InsertJob);
            const string Name = "BM18_0000001";

            JobItemDto dto = new JobItemDto
            {
                LongName = Name,
                Name = Name,
                Status = JobStatus.Compled,
                TargetDate = DateTime.Now
            };

            rule.Action(dto);

            using (var db = new DatabaseImpl())
            {
                Assert.Equal(1, db.Jobs.Count());
                Assert.Equal(Name, db.Jobs.Single().Id);
            }
        }
    }
}
