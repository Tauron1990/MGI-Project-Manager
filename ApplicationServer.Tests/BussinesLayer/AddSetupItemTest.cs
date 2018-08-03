using System;
using System.Linq;
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
    public class AddSetupItemTest : IDisposable
    {
        private readonly IContainer _container;
        private readonly InMemoryDatabaseRoot _inMemoryDatabaseRoot = new InMemoryDatabaseRoot();

        public AddSetupItemTest()
        {
            SetupApplication.AddSetupAction(() => DatabaseImpl.UpdateSchema(o => o.UseInMemoryDatabase("test", _inMemoryDatabaseRoot)));
            SetupApplication.AddTypes(typeof(MainDatabase));
            _container = SetupApplication.Setup(true, typeof(AddSetupItemRule).Assembly);
        }

        public void Dispose() => SetupApplication.Free();

        [Fact]
        public void TestMethod()
        {
            var addSetupRule = _container.GetIBusinessRule<AddSetupInput>(RuleNames.TimeCalculationAddSetupItems);
            var input = new AddSetupInput(new []
            {
                new AddSetupInputItem(new DateTime(2018, 1, 1, 10, 0, 0), new DateTime(2018, 1, 1, 10, 10, 0), RunTimeCalculatorItemType.Setup),
                new AddSetupInputItem(new DateTime(2018, 1, 1, 15, 0, 0), new DateTime(2018, 1, 1, 15, 10, 0), RunTimeCalculatorItemType.Iteration)
            });

            addSetupRule.Action(input);

            string msg = string.Join(", ", addSetupRule.Errors ?? new object[0]);
            Assert.False(addSetupRule.Error, $"AddRule Error On Valid Input: {msg}");

            using (var db = new DatabaseImpl())
                Assert.Equal(2, db.Setups.Count());
        }

    }
}
