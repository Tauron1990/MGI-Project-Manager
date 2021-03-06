﻿using Tauron.CQRS.Common.ServerHubs;

namespace Tauron.CQRS.Common.Dto
{
    public class ApiEventMessage
    {
        public string ApiKey { get; set; }

        public ServerDomainMessage[] DomainMessages { get; set; }
    }
}