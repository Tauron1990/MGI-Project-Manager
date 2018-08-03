using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Tauron.Application.Ioc;
using Tauron.Application.ProjectManager.ApplicationServer.BussinesLayer;
using Tauron.Application.ProjectManager.ApplicationServer.Data;
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


        }
    }
}
