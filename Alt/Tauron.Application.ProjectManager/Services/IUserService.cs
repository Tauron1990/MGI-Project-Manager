using System.Net.Security;
using System.ServiceModel;
using Tauron.Application.ProjectManager.Services.DTO;

namespace Tauron.Application.ProjectManager.Services
{
    [ServiceContract(CallbackContract = typeof(IUserServiceCallback), Namespace = "MGI-Proleckt-Server")]
    public interface IUserService
    {
        [OperationContract]
        [FaultContract(typeof(GenericServiceFault))]
        [FaultContract(typeof(LogInFault))]
        string[] GetUsers();

        [OperationContract(IsOneWay = true)]
        //[FaultContract(typeof(GenericServiceFault))]
        //[FaultContract(typeof(LogInFault))]
        void ChangePassword(string name, string newPassword, string oldPassword);

        [OperationContract]
        [FaultContract(typeof(GenericServiceFault))]
        [FaultContract(typeof(LogInFault))]
        UserRights GetUserRights(string user);
    }
}