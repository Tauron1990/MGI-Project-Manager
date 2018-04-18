using System.ServiceModel;
using Tauron.Application.ProjectManager.Services.DTO;

namespace Tauron.Application.ProjectManager.Services
{
    [ServiceContract]
    public interface IUserService
    {
        [OperationContract]
        [FaultContract(typeof(GenericServiceFault))]
        [FaultContract(typeof(LogInFault))]
        string[] GetUsers();

        [OperationContract]
        [FaultContract(typeof(GenericServiceFault))]
        [FaultContract(typeof(LogInFault))]
        GenericServiceResult ChangePassword(string name, string newPassword, string oldPassword);
    }
}