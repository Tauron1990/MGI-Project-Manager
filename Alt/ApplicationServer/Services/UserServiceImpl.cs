using System.ServiceModel;
using Tauron.Application.ProjectManager.ApplicationServer.Core;
using Tauron.Application.ProjectManager.Services;
using Tauron.Application.ProjectManager.Services.DTO;

namespace Tauron.Application.ProjectManager.ApplicationServer.Services
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Single)]
    public class UserServiceImpl : ServiceBase, IUserService
    {
        public string[] GetUsers() => Secure(UserManager.GetUsers);

        public void ChangePassword(string name, string newPassword, string oldPassword)
        {
            Secure(() =>
                          {
                              var ok = UserManager.ChangePassword(name, oldPassword, newPassword, out var reason);

                              OperationContext.Current.GetCallbackChannel<IUserServiceCallback>().PasswortChanged(new GenericServiceResult(ok, reason));
                          });
        }

        public UserRights GetUserRights(string user) => Secure(() => UserManager.GetUserRights(user));
    }
}