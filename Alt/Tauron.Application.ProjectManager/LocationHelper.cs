﻿using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using JetBrains.Annotations;

namespace Tauron.Application.ProjectManager
{
    [PublicAPI]
    public static class LocationHelper
    {
        public const string ApplicationServer = "MGI-Application-Server";

        public static Uri GetBaseAdress(string ip)
        {
            return new Uri($"net.tcp://{ip}/MGI-ProjectManager/{ApplicationServer}");
        }

        public static string BuildUrl(string ip, string serviceName)
        {
            return $"net.tcp://{ip}/MGI-ProjectManager/{ApplicationServer}/{serviceName}";
        }

        public static Binding CreateBinding()
        {
            return new NetTcpBinding
                   {
                       MaxReceivedMessageSize = 5 * 1024 * 1024,
                       Security = new NetTcpSecurity
                                  {
                                      Mode = SecurityMode.Message,
                                      Message = new MessageSecurityOverTcp
                                                {
                                                    ClientCredentialType = MessageCredentialType.UserName
                                                }
                                  },
                       ReliableSession = new OptionalReliableSession
                                         {
                                             Enabled = true,
                                             InactivityTimeout = TimeSpan.MaxValue
                                         }
                   };
        }
    }
}