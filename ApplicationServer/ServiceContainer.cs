using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Security;
using Tauron.Application.ProjectManager.ApplicationServer.Core;
using Tauron.Application.ProjectManager.ApplicationServer.Services;
using Tauron.Application.ProjectManager.Services;

namespace Tauron.Application.ProjectManager.ApplicationServer
{
    internal class ServiceContainer
    {
        private static readonly (Type, Type, string)[] ServiceTypes =
        {
            (typeof(AdminServiceImpl), typeof(IAdminService), ServiceNames.AdminService),
            (typeof(UserServiceImpl), typeof(IUserService), ServiceNames.UserService)
        }; 

        private readonly List<ServiceHost> _services = new List<ServiceHost>();

        public void Start(IpSettings settings)
        {
            IpSettings.WriteIpSettings(settings);

            Uri baseAddress = LocationHelper.GetBaseAdress(settings.NetworkTarget);

            foreach (var serviceType in ServiceTypes)
            {
                ServiceHost host = new ServiceHost(serviceType.Item1, baseAddress);

                host.Credentials.UserNameAuthentication.UserNamePasswordValidationMode = UserNamePasswordValidationMode.Custom;
                host.Credentials.UserNameAuthentication.CustomUserNamePasswordValidator = new UserAuthentication();

                host.AddServiceEndpoint(serviceType.Item2, LocationHelper.CreateBinding(), serviceType.Item3);
                
                host.Open();
                _services.Add(host);
            }
        }

        public void Stop()
        {
            foreach (var serviceHost in _services)
                serviceHost.Close();

            _services.Clear();
        }
    }
}