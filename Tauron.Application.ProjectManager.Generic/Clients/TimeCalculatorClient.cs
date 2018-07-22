using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Tauron.Application.ProjectManager.Services;
using Tauron.Application.ProjectManager.Services.DTO;

namespace Tauron.Application.ProjectManager.Generic.Clients
{
    public sealed class TimeCalculatorClient : ClientHelperBase<ITimeCalculator>, ITimeCalculator
    {
        public TimeCalculatorClient(Binding binding, EndpointAddress adress) : base(binding, adress)
        {
        }

        public ValidationOutput InsertValidation(ValidationInput validationInput) => Secure(() => Channel.InsertValidation(validationInput));

        public SaveOutput Save(SaveInput saveInput) => Secure(() => Channel.Save(saveInput));

        public void AddSetupItems(AddSetupInput addSetupInput) => Channel.AddSetupItems(addSetupInput);

        public void RecalculateSetup() => Secure(() => Channel.RecalculateSetup());

        public CalculateValidateOutput CalculateValidation(CalculateTimeInput calculateTimeInput) => Secure(() => Channel.CalculateValidation(calculateTimeInput));

        public CalculateTimeOutput CalculateTime(CalculateTimeInput calculateTimeInput) => Secure(() => Channel.CalculateTime(calculateTimeInput));

        public JobRunDto[] FetchJobInfo(JobItemDto createDto) => Secure(() => Channel.FetchJobInfo(createDto));
    }
}