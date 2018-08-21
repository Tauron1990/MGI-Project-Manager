using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Tauron.Application.Ioc;
using Tauron.Application.ProjectManager.ApplicationServer.BussinesLayer;
using Tauron.Application.ProjectManager.ApplicationServer.Core;
using Tauron.Application.ProjectManager.ApplicationServer.Data;
using Tauron.Application.ProjectManager.Services.Data.Entitys;
using TestHelperLib;
using Xunit;

namespace ApplicationServer.Tests.BussinesLayer
{
    public class TimeCalculationRecalculateSetupRuleTest : IDisposable
    {
        private readonly IContainer _container;
        private readonly InMemoryDatabaseRoot _inMemoryDatabaseRoot = new InMemoryDatabaseRoot();

        public TimeCalculationRecalculateSetupRuleTest()
        {
            SetupApplication.AddSetupAction(() => DatabaseImpl.UpdateSchema(o => o.UseInMemoryDatabase("test", _inMemoryDatabaseRoot)));
            SetupApplication.AddTypes(typeof(MainDatabase));
            _container = SetupApplication.Setup(true, typeof(AddSetupItemRule).Assembly);
        }

        public void Dispose() => SetupApplication.Free();

        [Fact]
        public void Test()
        {
            using (var db = new DatabaseImpl())
            {
                for (int i = 0; i < 2; i++)
                {
                    for (int j = 0; j < 2; j++)
                    {
                        db.Setups.Add(new SetupEntity
                        {
                            SetupType = SetupType.Setup,
                            StartTime = DateTime.Now + TimeSpan.FromHours(j),
                            Value = j == 0 ? 10 : 30
                        });
                    }

                    for (int j = 0; j < 2; j++)
                    {
                        db.Setups.Add(new SetupEntity
                        {
                            SetupType = SetupType.Iteration,
                            StartTime = DateTime.Now + TimeSpan.FromHours(j),
                            Value = j == 0 ? 10 : 30
                        });
                    }
                }

                db.SaveChanges();
            }

            var rule = _container.GetBusinessRule(RuleNames.TimeCalculationRecalculateSetup);
            rule.Action();

            Assert.Equal(20, SettingsManager.Default.IterationTime);
            Assert.Equal(20, SettingsManager.Default.SetupTime);
        }
    }
}