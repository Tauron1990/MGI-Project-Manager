using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Tauron.Application.Ioc;
using Tauron.Application.ProjectManager.ApplicationServer.BussinesLayer;
using Tauron.Application.ProjectManager.ApplicationServer.Data;
using TestHelperLib;
using Xunit;

namespace ApplicationServer.Tests.BussinesLayer
{
    public class TimeCalculationCalculationValidationRuleTest : IDisposable
    {
        private readonly IContainer _container;
        private readonly InMemoryDatabaseRoot _inMemoryDatabaseRoot = new InMemoryDatabaseRoot();

        public TimeCalculationCalculationValidationRuleTest()
        {
            SetupApplication.AddSetupAction(() =>
                DatabaseImpl.UpdateSchema(o => o.UseInMemoryDatabase("test", _inMemoryDatabaseRoot)));
            SetupApplication.AddTypes(typeof(MainDatabase));
            _container = SetupApplication.Setup(true, typeof(AddSetupItemRule).Assembly);
        }

        public void Dispose() => SetupApplication.Free();

        [Fact]
        public void Test()
        {

        }

        private 
    }
}