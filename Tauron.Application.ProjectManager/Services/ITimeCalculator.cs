using System.ServiceModel;
using Tauron.Application.ProjectManager.Services.DTO;

namespace Tauron.Application.ProjectManager.Services
{
    [ServiceContract(Namespace = "MGI-Project-Server")]
    public interface ITimeCalculator
    {
        [OperationContract]
        ValidationOutput        InsertValidation(ValidationInput validationInput);
        [OperationContract]
        SaveOutput              Save(SaveInput                   saveInput);
        [OperationContract]
        void                    AddSetupItems(AddSetupInput      addSetupInput);
        [OperationContract]
        void                    RecalculateSetup();
        [OperationContract]
        CalculateValidateOutput CalculateValidation(CalculateTimeInput calculateTimeInput);
        [OperationContract]
        CalculateTimeOutput     CalculateTime(CalculateTimeInput       calculateTimeInput);
        [OperationContract]
        JobRunDto[]               FetchJobInfo(JobItemDto                createDto);
    }
}