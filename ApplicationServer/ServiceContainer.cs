using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Security;
using Tauron.Application.ProjectManager.ApplicationServer.Core;
using Tauron.Application.ProjectManager.ApplicationServer.Data;
using Tauron.Application.ProjectManager.ApplicationServer.Services;
using Tauron.Application.ProjectManager.Services;

namespace Tauron.Application.ProjectManager.ApplicationServer
{
    internal class ServiceContainer
    {
        private static readonly (Type ServiceImpl, Type SeriviceInterface, string ServiceName)[] ServiceTypes =
        {
            (typeof(AdminServiceImpl), typeof(IAdminService), ServiceNames.AdminService),
            (typeof(UserServiceImpl), typeof(IUserService), ServiceNames.UserService),
            (typeof(JobManagerImpl), typeof(IJobManager), ServiceNames.JobManager),
            (typeof(JobPushMessageImpl), typeof(IJobPushMessage), ServiceNames.JobPushMessage),
            (typeof(TimeCalculatorImpl), typeof(ITimeCalculator), ServiceNames.TimeCalculator)
        };

        private readonly List<ServiceHost> _services = new List<ServiceHost>();

        public void Start(IpSettings settings)
        {
            var str = new SecureString();
            foreach (var c in "tauron")
            {
                str.AppendChar(c);
            }

            IpSettings.WriteIpSettings(settings);

            var baseAddress = LocationHelper.GetBaseAdress(settings.NetworkTarget);

            foreach (var serviceType in ServiceTypes)
            {
                var host = new ServiceHost(serviceType.ServiceImpl, baseAddress);

                host.Faulted += HostOnFaulted;

                host.Credentials.UserNameAuthentication.UserNamePasswordValidationMode       = UserNamePasswordValidationMode.Custom;
                host.Credentials.UserNameAuthentication.CustomUserNamePasswordValidator      = new UserAuthentication();
                host.Credentials.ServiceCertificate.Certificate                              = new X509Certificate2(Properties.Resources.local, str);
                host.Credentials.ClientCertificate.Certificate                               = new X509Certificate2(Properties.Resources.local2, str);
                host.Credentials.ClientCertificate.Authentication.CertificateValidationMode  = X509CertificateValidationMode.Custom;
                host.Credentials.ClientCertificate.Authentication.CustomCertificateValidator = new CertificateValidator();

                host.AddServiceEndpoint(serviceType.SeriviceInterface, LocationHelper.CreateBinding(), serviceType.ServiceName);

                host.Open();

                _services.Add(host);
            }
        }

        private static void HostOnFaulted(object sender, EventArgs e)
        {
            Bootstrapper.FaultedStop();
        }

        public void Stop()
        {
            ConnectivityManager.Close();

            foreach (var serviceHost in _services)
            {
                try
                {
                    serviceHost.Close(TimeSpan.FromMinutes(5));
                }
                catch (Exception e) when(e is CommunicationException || e is TimeoutException)
                {
                    serviceHost.Abort();
                }
            }

            _services.Clear();
        }
    }
}