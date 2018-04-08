using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using JetBrains.Annotations;

namespace Tauron.Application.ProjectManager
{
    [PublicAPI]
    public static class LocationHelper
    {
        public const string ApplicationServer = "MGI-Application-Server";

        public static Uri GetBaseAdress(string ip) => new Uri($"net.tcp://{ip}/MGI-ProjectManager/{ApplicationServer}");
        public static string BuildUrl(string ip, string serviceName) => $"net.tcp://{ip}/MGI-ProjectManager/{ApplicationServer}/{serviceName}";

        public static Binding CreateBinding() => new NetTcpBinding
                                                 {
                                                     MaxReceivedMessageSize =  5 * 1024 * 1024, 
                                                     Security = new NetTcpSecurity
                                                                {
                                                                    Mode = SecurityMode.TransportWithMessageCredential,
                                                                    Message = new MessageSecurityOverTcp
                                                                              {
                                                                                  ClientCredentialType = MessageCredentialType.UserName,
                                                                              }
                                                                }
                                                 };
    }
}