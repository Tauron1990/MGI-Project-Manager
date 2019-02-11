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

        public bool IsAdminPasswordNull()
        {
            return Secure(() =>
                          {
                              CheckAdmin();
                              return UserManager.HasNoPassword("admin");
                          });
        }

        public GenericServiceResult CreateUser(string userName, string password)
        {
            return Secure(() =>
                          {
                              CheckAdmin();
                              var ok = UserManager.CreateUser(userName, password, out var reason);

                              return new GenericServiceResult(ok, reason);
                          });
        }

        public GenericServiceResult DeleteUser(string userName)
        {
            return Secure(() =>
                          {
                              CheckAdmin();

                              var ok = UserManager.DeleteUser(userName, out var reason);

                              return new GenericServiceResult(ok, reason);
                          });
        }

        public void SetUserRights(string name, UserRights rights)
        {
            Secure(() =>
                   {
                       CheckAdmin();
                       UserManager.SetUserRights(name, rights);
                   });
        }

        public void AdminLogout()
        {
            _adminGrandet = false;
            Logger.Info("Admin - Logout");
        }

        private void CheckAdmin()
        {
            if (_adminGrandet) return;

            throw new FaultException<GenericServiceFault>(new GenericServiceFault(typeof(LogInFault), "LogIn"));
        }
    }
}