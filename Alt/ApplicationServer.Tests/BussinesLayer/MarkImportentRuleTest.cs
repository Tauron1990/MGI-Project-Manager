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
    public class MarkImportentRuleTest : IDisposable
    {
        private readonly IContainer _container;
        private readonly InMemoryDatabaseRoot _inMemoryDatabaseRoot = new InMemoryDatabaseRoot();

        public MarkImportentRuleTest()
        {
            SetupApplication.AddSetupAction(() => DatabaseImpl.UpdateSchema(o => o.UseInMemoryDatabase("test", _inMemoryDatabaseRoot)));
            SetupApplication.AddTypes(typeof(MainDatabase));
            _container = SetupApplication.Setup(true, typeof(AddSetupItemRule).Assembly);
        }

        public void Dispose() => SetupApplication.Free();

        [Fact]
        public void Test()
        {
            const string name = "BM18_00001";
            var ent = new JobEntity {Id = name, Importent = false, LongName = name};

            using (var db = new DatabaseImpl())
            {
                db.Jobs.Add(ent);
                db.SaveChanges();
            }

            var rule = _container.GetIBusinessRule<JobItemDto>(RuleNames.MarkImportent);

            rule.Action(JobItemDto.FromEntity(ent));

            using (var db = new DatabaseImpl())
                Assert.Equal(name, db.Jobs.Single().Id);
        }
    }
}
