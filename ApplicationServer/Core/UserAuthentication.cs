using System;
using System.IdentityModel.Selectors;
using System.ServiceModel;
using Tauron.Application.ProjectManager.Services.DTO;

namespace Tauron.Application.ProjectManager.ApplicationServer.Core
{
    public class UserAuthentication : UserNamePasswordValidator
    {
        public override void Validate(string userName, string password)
        {
            try
            {
                if (UserManager.Validate(userName, password, out var reason)) return;

                throw new FaultException<LogInFault>(new LogInFault(reason));
            }
            catch (Exception e)
            {
                if (CriticalExceptions.IsCriticalException(e)) throw;

                throw new FaultException(new FaultReason($"{e.GetType()} - {e.Message}"));
            }
        }
    }
}