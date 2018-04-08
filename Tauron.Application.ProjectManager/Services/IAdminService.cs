using System.ServiceModel;
using Tauron.Application.ProjectManager.Services.DTO;

namespace Tauron.Application.ProjectManager.Services
{
    [ServiceContract(SessionMode = SessionMode.Allowed)]
    public interface IAdminService
    {
        [OperationContract(IsInitiating = true)]
        [FaultContract(typeof(GenericServiceFault)), FaultContract(typeof(LogInFault))]
        void AdminLogin(string password);

        [OperationContract]
        [FaultContract(typeof(GenericServiceFault)), FaultContract(typeof(LogInFault))]
        GenericServiceResult CreateUser(string userName, string password);

        [OperationContract]
        [FaultContract(typeof(GenericServiceFault)), FaultContract(typeof(LogInFault))]
        GenericServiceResult DeleteUser(string userName);

        [OperationContract(IsTerminating = true)]
        [FaultContract(typeof(GenericServiceFault)), FaultContract(typeof(LogInFault))]
        void AdminLogout();
    }
}