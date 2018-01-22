using System;
using Tauron.Application.Common.BaseLayer.BusinessLayer;
using Tauron.Application.Ioc;
using Tauron.Application.MgiProjectManager.BussinesLayer;
using Tauron.Application.MgiProjectManager.ServiceLayer.Dto;

namespace Tauron.Application.MgiProjectManager.ServiceLayer.Impl
{
    [Export(typeof(ITimeCalculator))]
    public sealed class TimeCalculator : ITimeCalculator
    {
        [Inject]
        private RuleFactory _ruleFactory;

        public ValidationOutput InsertValidation(ValidationInput validationInput)
        {
            return ExecutionHelper.ExecuteRule(_ruleFactory.CreateIioBusinessRule<ValidationInput, ValidationOutput>(RuleNames.TimeCalculationValidation),
                                               validationInput, null, nameof(ITimeCalculator));
        }

        public SaveOutput Save(SaveInput saveInput)
        {
            return ExecutionHelper.ExecuteRule(_ruleFactory.CreateIioBusinessRule<SaveInput, SaveOutput>(RuleNames.TimeCalculationSaving),
                                               saveInput, new SaveOutput(false, null), nameof(ITimeCalculator));
        }

        public void AddSetupItems(AddSetupInput addSetupInput)
        {
            ExecutionHelper.ExecuteRule(_ruleFactory.CreateIiBusinessRule<AddSetupInput>(RuleNames.TimeCalculationAddSetupItems), addSetupInput, nameof(ITimeCalculator));
        }

        public void RecalculateSetup()
        {
            ExecutionHelper.ExecuteRule(_ruleFactory.CreateBusinessRule(RuleNames.TimeCalculationRecalculateSetup), nameof(ITimeCalculator));
        }

        public CalculateValidateOutput CalculateValidation(CalculateTimeInput calculateTimeInput)
        {
            return ExecutionHelper.ExecuteRule(_ruleFactory.CreateIioBusinessRule<CalculateTimeInput, CalculateValidateOutput>(RuleNames.TimeCalculationCalculationValidation), calculateTimeInput,
                                               new CalculateValidateOutput(false, string.Empty), nameof(ITimeCalculator));
        }

        public CalculateTimeOutput CalculateTime(CalculateTimeInput calculateTimeInput)
        {
            return ExecutionHelper.ExecuteRule(_ruleFactory.CreateIioBusinessRule<CalculateTimeInput, CalculateTimeOutput>(RuleNames.TimeCalculationCalculateTime), calculateTimeInput,
                                               new CalculateTimeOutput(null, null, null, string.Empty, PrecisionMode.InValid), nameof(ITimeCalculator));
        }

        public JobRunDto FetchJobInfo(JobItemDto createDto)
        {
            return ExecutionHelper.ExecuteRule(_ruleFactory.CreateIioBusinessRule<JobItemDto, JobRunDto>(RuleNames.TimeCalculationFetchJobInfo), createDto,
                                               new JobRunDto(0, 0, 0, 0, 0, TimeSpan.Zero, false), nameof(ITimeCalculator));
        }
    }
}