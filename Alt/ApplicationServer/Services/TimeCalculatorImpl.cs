using System;
using System.ServiceModel;
using Tauron.Application.Common.BaseLayer.BusinessLayer;
using Tauron.Application.Ioc;
using Tauron.Application.ProjectManager.ApplicationServer.BussinesLayer;
using Tauron.Application.ProjectManager.ApplicationServer.Core;
using Tauron.Application.ProjectManager.Services;
using Tauron.Application.ProjectManager.Services.DTO;

namespace Tauron.Application.ProjectManager.ApplicationServer.Services
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public sealed class TimeCalculatorImpl : ServiceBase, ITimeCalculator
    {
        public TimeCalculatorImpl()
        {
            AllowedRights = UserRights.Operater;
        }

        [Inject]
        private RuleFactory _ruleFactory;

        public ValidationOutput InsertValidation(ValidationInput validationInput)
        {
            return ExecuteRule(_ruleFactory.CreateIioBusinessRule<ValidationInput, ValidationOutput>(RuleNames.TimeCalculationValidation),
                                               validationInput, null, nameof(ITimeCalculator));
        }

        public SaveOutput Save(SaveInput saveInput)
        {
            return ExecuteRule(_ruleFactory.CreateIioBusinessRule<SaveInput, SaveOutput>(RuleNames.TimeCalculationSaving),
                                               saveInput, new SaveOutput(false, null), nameof(ITimeCalculator));
        }

        public void AddSetupItems(AddSetupInput addSetupInput)
        {
            ExecuteRule(_ruleFactory.CreateIiBusinessRule<AddSetupInput>(RuleNames.TimeCalculationAddSetupItems), addSetupInput, nameof(ITimeCalculator));
        }

        public void RecalculateSetup()
        {
            ExecuteRule(_ruleFactory.CreateBusinessRule(RuleNames.TimeCalculationRecalculateSetup), nameof(ITimeCalculator));
        }

        public CalculateValidateOutput CalculateValidation(CalculateTimeInput calculateTimeInput)
        {
            return ExecuteRule(_ruleFactory.CreateIioBusinessRule<CalculateTimeInput, CalculateValidateOutput>(RuleNames.TimeCalculationCalculationValidation), calculateTimeInput,
                                               new CalculateValidateOutput(false, string.Empty), nameof(ITimeCalculator));
        }

        public CalculateTimeOutput CalculateTime(CalculateTimeInput calculateTimeInput)
        {
            return ExecuteRule(_ruleFactory.CreateIioBusinessRule<CalculateTimeInput, CalculateTimeOutput>(RuleNames.TimeCalculationCalculateTime), calculateTimeInput,
                                               new CalculateTimeOutput(null, null, null, string.Empty, PrecisionMode.InValid), nameof(ITimeCalculator));
        }

        public JobRunDto[] FetchJobInfo(JobItemDto createDto)
        {
            return ExecuteRule(_ruleFactory.CreateIioBusinessRule<JobItemDto, JobRunDto[]>(RuleNames.TimeCalculationFetchJobInfo), createDto,
                                               new [] {new JobRunDto(string.Empty, 0, 0, 0, 0, 0, TimeSpan.Zero, false)}, nameof(ITimeCalculator));
        }
    }
}