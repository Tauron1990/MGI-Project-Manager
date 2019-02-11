using System.Net.Security;
using System.ServiceModel;
using Tauron.Application.ProjectManager.Services.DTO;

namespace Tauron.Application.ProjectManager.Services
{
    public interface IUserServiceCallback
    {
        [OperationContract(IsOneWay = true)]
        void PasswortChanged(GenericServiceResult result);
    }
}