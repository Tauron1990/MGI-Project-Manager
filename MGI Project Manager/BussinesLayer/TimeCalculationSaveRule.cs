using System.Linq;
using Microsoft.EntityFrameworkCore;
using Tauron.Application.Common.BaseLayer;
using Tauron.Application.Common.BaseLayer.BusinessLayer;
using Tauron.Application.Common.BaseLayer.Core;
using Tauron.Application.Common.BaseLayer.Data;
using Tauron.Application.MgiProjectManager.Data.Entitys;
using Tauron.Application.MgiProjectManager.ServiceLayer.Dto;

namespace Tauron.Application.MgiProjectManager.BussinesLayer
{
    [ExportRule(RuleNames.TimeCalculationSaving)]
    public sealed class TimeCalculationSaveRule : IOBusinessRuleBase<SaveInput, SaveOutput>
    {
        [InjectRuleFactory]
        private RuleFactory _ruleFactory;

        public override SaveOutput ActionImpl(SaveInput input)
        {
            var normalizedResult = _ruleFactory.CreateIioBusinessRule<ValidationInput, ValidationOutput>(RuleNames.TimeCalculationValidation)
                                               .Action(new ValidationInput(input.Amount, input.Iteratins, input.RunTime, input.PaperFormat, input.Speed));

            if (normalizedResult.NormalizedTime == null) return new SaveOutput(false, normalizedResult.FormatedResult);

            using (var database = RepositoryFactory.Enter())
            {
                var repo   = RepositoryFactory.GetRepository<Repository<JobEntity, string>>();
                var jobrun = repo.Query().Include(e => e.JobRun).Single(e => e.Id == input.JobItem.Name).JobRun;

                // ReSharper disable PossibleInvalidOperationException
                jobrun.Amount        = input.Amount.Value;
                jobrun.BigProblem    = input.BigProblem;
                jobrun.Problem       = input.Problem;
                jobrun.EffectiveTime = input.RunTime;
                jobrun.Iterations    = input.Iteratins.Value;
                jobrun.Length        = input.PaperFormat.Lenght.Value;
                jobrun.NormaizedTime = normalizedResult.NormalizedTime.Value;
                jobrun.Speed         = input.Speed.Value;
                jobrun.StartTime     = input.StartTime;
                jobrun.Width         = input.PaperFormat.Width.Value;
                // ReSharper restore PossibleInvalidOperationException
                jobrun.IterationTime = input.IterationTime;
                jobrun.SetupTime     = input.SetupTime;
                jobrun.IsValid       = true;

                database.SaveChanges();
            }

            return new SaveOutput(true, string.Empty);
        }
    }
}