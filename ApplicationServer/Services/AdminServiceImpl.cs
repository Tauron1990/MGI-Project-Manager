using System;
using System.ServiceModel;
using NLog;
using Tauron.Application.ProjectManager.ApplicationServer.Core;
using Tauron.Application.ProjectManager.Services;
using Tauron.Application.ProjectManager.Services.DTO;

namespace Tauron.Application.ProjectManager.ApplicationServer.Services
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)]
    public sealed class AdminServiceImpl : ServiceBase, IAdminService
    {
        private bool _adminGrandet;

        public void AdminLogin(string password)
        {
            if (!UserManager.Validate("admin", password, out var reason))
                throw new FaultException<GenericServiceFault>(new GenericServiceFault(typeof(LogInFault), reason));

            _adminGrandet = true;

            Logger.Log(LogLevel.Info, $"Admin Login - {DateTime.Today}");
        }

        public GenericServiceResult CreateUser(string userName, string password)
        {
            return Secure(() =>
                          {
                              CheckAdmin();
                              bool ok = UserManager.CreateUser(userName, password, out var reason);

                              return new GenericServiceResult(ok, reason);
                          });
        }

        public GenericServiceResult DeleteUser(string userName)
        {
            return Secure(() =>
                          {
                              CheckAdmin();

                              bool ok = UserManager.DeleteUser(userName, out var reason);

                              return new GenericServiceResult(ok, reason);
                          });
        }

        public void AdminLogout() => _adminGrandet = false;

        private void CheckAdmin()
        {
            if (_adminGrandet) return;

            throw new FaultException<GenericServiceFault>(new GenericServiceFault(typeof(LogInFault), "LogIn"));
        }
    }
}