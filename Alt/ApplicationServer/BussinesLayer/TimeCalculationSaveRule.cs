using System.Linq;
using Microsoft.EntityFrameworkCore;
using Tauron.Application.Common.BaseLayer;
using Tauron.Application.Common.BaseLayer.BusinessLayer;
using Tauron.Application.Common.BaseLayer.Core;
using Tauron.Application.ProjectManager.ApplicationServer.Data.Repositorys;
using Tauron.Application.ProjectManager.Services.Data.Entitys;
using Tauron.Application.ProjectManager.Services.DTO;

namespace Tauron.Application.ProjectManager.ApplicationServer.BussinesLayer
{
    [ExportRule(RuleNames.TimeCalculationSaving)]
    public sealed class TimeCalculationSaveRule : IOBusinessRuleBase<SaveInput, SaveOutput>
    {
        [InjectRuleFactory]
        private RuleFactory _ruleFactory;

        public override SaveOutput ActionImpl(SaveInput input)
        {
            var normalizedResult = _ruleFactory.CreateIioBusinessRule<ValidationInput, ValidationOutput>(RuleNames.TimeCalculationValidation)
                                               .Action(new ValidationInput(input.JobItem.Name, input.Amount, input.Iteratins, input.RunTime, input.PaperFormat, input.Speed));

            if (normalizedResult.NormalizedTime == null) return new SaveOutput(false, normalizedResult.FormatedResult);

            using (var database = RepositoryFactory.Enter())
            {
                var repo   = RepositoryFactory.GetRepository<IJobRepository>();
                var jobruns = repo.Query().Include(e => e.JobRuns).Single(e => e.Id == input.JobItem.Name).JobRuns;

                // ReSharper disable PossibleInvalidOperationException
                JobRunEntity jobrun = new JobRunEntity
                {
                    Amount = input.Amount.Value,
                    BigProblem = input.BigProblem,
                    Problem = input.Problem,
                    EffectiveTime = input.RunTime,
                    Iterations = input.Iteratins.Value,
                    Length = input.PaperFormat.Lenght.Value,
                    NormaizedTime = normalizedResult.NormalizedTime.Value,
                    Speed = input.Speed.Value,
                    StartTime = input.StartTime,
                    Width = input.PaperFormat.Width.Value,
                    IterationTime = input.IterationTime,
                    SetupTime = input.SetupTime,
                    IsSaved = true
                };
                // ReSharper restore PossibleInvalidOperationException
                
                jobruns.Add(jobrun);
                database.SaveChanges();
            }

            return new SaveOutput(true, string.Empty);
        }
    }
}