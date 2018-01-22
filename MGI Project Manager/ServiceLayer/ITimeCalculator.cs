using Tauron.Application.MgiProjectManager.ServiceLayer.Dto;

namespace Tauron.Application.MgiProjectManager.ServiceLayer
{
    public interface ITimeCalculator
    {
        ValidationOutput        InsertValidation(ValidationInput validationInput);
        SaveOutput              Save(SaveInput                   saveInput);
        void                    AddSetupItems(AddSetupInput      addSetupInput);
        void                    RecalculateSetup();
        CalculateValidateOutput CalculateValidation(CalculateTimeInput calculateTimeInput);
        CalculateTimeOutput     CalculateTime(CalculateTimeInput       calculateTimeInput);
        JobRunDto               FetchJobInfo(JobItemDto                createDto);
    }
}