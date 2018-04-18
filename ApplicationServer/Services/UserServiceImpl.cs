using System.Linq;
using System.ServiceModel;
using Microsoft.EntityFrameworkCore;
using Tauron.Application.ProjectManager.ApplicationServer.Core;
using Tauron.Application.ProjectManager.ApplicationServer.Data;
using Tauron.Application.ProjectManager.Services;
using Tauron.Application.ProjectManager.Services.DTO;

namespace Tauron.Application.ProjectManager.ApplicationServer.Services
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class UserServiceImpl : ServiceBase, IUserService
    {
        public string[] GetUsers()
        {
            return Secure(() =>
                          {
                              using (var db = new DatabaseImpl())
                                  return db.Users.AsNoTracking().Select(u => u.Id).ToArray();
                          });
        }

        public GenericServiceResult ChangePassword(string name, string newPassword, string oldPassword)
        {
            return Secure(() =>
                          {
                              var ok = UserManager.ChangePassword(name, oldPassword, newPassword, out var reason);

                              return new GenericServiceResult(ok, reason);
                          });
        }
    }
}