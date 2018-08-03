using System;
using System.Collections.Generic;
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
    public class GetActiveJobsRuleTest : IDisposable
    {
        private readonly IContainer _container;
        private readonly InMemoryDatabaseRoot _inMemoryDatabaseRoot = new InMemoryDatabaseRoot();

        public GetActiveJobsRuleTest()
        {
            SetupApplication.AddSetupAction(() => DatabaseImpl.UpdateSchema(o => o.UseInMemoryDatabase("test", _inMemoryDatabaseRoot)));
            SetupApplication.AddTypes(typeof(MainDatabase));
            _container = SetupApplication.Setup(true, typeof(AddSetupItemRule).Assembly);
        }

        public void Dispose() => SetupApplication.Free();

        [Fact]
        public void TestMethod()
        {
            using (var impl = new DatabaseImpl())
            {
                impl.Jobs.Add(new JobEntity {IsActive = true, Id = "1", LongName = "1", Status = JobStatus.Pending});
                impl.Jobs.Add(new JobEntity { IsActive = false, Id = "2", LongName = "2", Status = JobStatus.Compled });
                impl.SaveChanges();
            }

            var rule = _container.GetOBusinessRule<IEnumerable<JobItemDto>>(RuleNames.GetActiveJobsRule);

            JobItemDto[] erg = rule.Action().ToArray();

            Assert.Single(erg, dto => dto.Name == "1" && dto.LongName == "1" && dto.Status == JobStatus.Pending);
        }
    }
}