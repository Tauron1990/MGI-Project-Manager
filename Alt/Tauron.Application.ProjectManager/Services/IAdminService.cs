using System.ServiceModel;
using Tauron.Application.ProjectManager.Services.DTO;

namespace Tauron.Application.ProjectManager.Services
{
    [ServiceContract(SessionMode = SessionMode.Required, Namespace = "MGI-Proleckt-Server")]
    public interface IAdminService
    {
        [OperationContract(IsInitiating = true)]
        [FaultContract(typeof(GenericServiceFault))]
        [FaultContract(typeof(LogInFault))]
        void AdminLogin(string password);

        [OperationContract]
        [FaultContract(typeof(GenericServiceFault))]
        [FaultContract(typeof(LogInFault))]
        bool IsAdminPasswordNull();

        [OperationContract]
        [FaultContract(typeof(GenericServiceFault))]
        [FaultContract(typeof(LogInFault))]
        GenericServiceResult CreateUser(string userName, string password);

        [OperationContract]
        [FaultContract(typeof(GenericServiceFault))]
        [FaultContract(typeof(LogInFault))]
        GenericServiceResult DeleteUser(string userName);

        [OperationContract]
        [FaultContract(typeof(GenericServiceFault))]
        [FaultContract(typeof(LogInFault))]
        void SetUserRights(string name, UserRights rights);

        [OperationContract(IsTerminating = true)]
        [FaultContract(typeof(GenericServiceFault))]
        [FaultContract(typeof(LogInFault))]
        void AdminLogout();
    }
}