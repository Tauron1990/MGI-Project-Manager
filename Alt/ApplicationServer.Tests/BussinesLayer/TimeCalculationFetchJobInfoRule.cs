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
    public class TimeCalculationFetchJobInfoRuleTest : IDisposable
    {
        private readonly IContainer _container;
        private readonly InMemoryDatabaseRoot _inMemoryDatabaseRoot = new InMemoryDatabaseRoot();

        public TimeCalculationFetchJobInfoRuleTest
            ()
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
            ent.JobRuns.Add(new JobRunEntity
            {
                Amount = 1000,
                BigProblem = true,
                EffectiveTime = TimeSpan.FromMinutes(60),
                IsCompleted = true,
                IsSaved = true,
                Iterations = 0,
                IterationTime = 20,
                Length = 32,
                NormaizedTime = TimeSpan.FromMinutes(60),
                Problem = true,
                SetupTime = 20,
                Speed = 0.7,
                StartTime = DateTime.Now,
                Width = 32,
            });
            ent.JobRuns.Add(new JobRunEntity
            {
                Amount = 1000,
                BigProblem = true,
                EffectiveTime = TimeSpan.FromMinutes(60),
                IsCompleted = true,
                IsSaved = true,
                Iterations = 0,
                IterationTime = 20,
                Length = 32,
                NormaizedTime = TimeSpan.FromMinutes(60),
                Problem = true,
                SetupTime = 20,
                Speed = 0.7,
                StartTime = DateTime.Now,
                Width = 32,
            });
            ent.JobRuns.Add(new JobRunEntity
            {
                Amount = 1000,
                BigProblem = true,
                EffectiveTime = TimeSpan.FromMinutes(60),
                IsCompleted = true,
                IsSaved = true,
                Iterations = 0,
                IterationTime = 20,
                Length = 32,
                NormaizedTime = TimeSpan.FromMinutes(60),
                Problem = true,
                SetupTime = 20,
                Speed = 0.7,
                StartTime = DateTime.Now,
                Width = 32,
            });
            ent.JobRuns.Add(new JobRunEntity
            {
                Amount = 1000,
                BigProblem = true,
                EffectiveTime = TimeSpan.FromMinutes(60),
                IsCompleted = true,
                IsSaved = true,
                Iterations = 0,
                IterationTime = 20,
                Length = 32,
                NormaizedTime = TimeSpan.FromMinutes(60),
                Problem = true,
                SetupTime = 20,
                Speed = 0.7,
                StartTime = DateTime.Now,
                Width = 32,
            });

            using (var db = new DatabaseImpl())
            {
                db.Jobs.Add(ent);
                db.SaveChanges();
            }

            var rule = _container.GetIoBusinessRule<JobItemDto, JobRunDto[]>(RuleNames.TimeCalculationFetchJobInfo);

            var erg = rule.Action(JobItemDto.FromEntity(ent));

            Assert.Equal(4, erg.Length);
        }
    }
}
