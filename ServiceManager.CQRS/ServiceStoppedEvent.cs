using System;
using Tauron.CQRS.Common.ServerHubs;
using Tauron.CQRS.Services.Core;

namespace ServiceManager.CQRS
{
    public class ServiceStoppedEvent : CQRSEvent
    {
        private static readonly Guid Namespace = Guid.Parse("F26B69D6-B39A-4E9E-B445-064305D118C1");

        public string ServiceName { get; }

        public ServiceStoppedEvent(string serviceName) 
            : base(IdGenerator.Generator.NewGuid(Namespace, serviceName), 0)
        {
            ServiceName = serviceName;
        }
    }
}