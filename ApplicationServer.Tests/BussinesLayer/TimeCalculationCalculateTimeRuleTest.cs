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
    public class TimeCalculationCalculateTimeRuleTest : IDisposable
    {
        private readonly IContainer _container;
        private readonly InMemoryDatabaseRoot _inMemoryDatabaseRoot = new InMemoryDatabaseRoot();

        public TimeCalculationCalculateTimeRuleTest()
        {
            SetupApplication.AddSetupAction(() => DatabaseImpl.UpdateSchema(o => o.UseInMemoryDatabase("test", _inMemoryDatabaseRoot)));
            SetupApplication.AddTypes(typeof(MainDatabase));
            _container = SetupApplication.Setup(true, typeof(AddSetupItemRule).Assembly);
        }

        public void Dispose() => SetupApplication.Free();

        [Fact]
        public void Test()
        {
            var rule = _container.GetIoBusinessRule<CalculateTimeInput, CalculateTimeOutput>(RuleNames
                .TimeCalculationCalculateTime);

            var erg = rule.Action(new CalculateTimeInput("BM18_00001", 1, new PaperFormat(10, 100), 1000, 0.7));

            Assert.Equal(PrecisionMode.NoData, erg.PrecisionMode);
        }
    }
}