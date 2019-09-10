using System;

namespace Tauron.CQRS.Common.Dto
{
    public class ApiEventId
    {
        public Guid Id { get; set; }

        public int Version { get; set; }

        public string ApiKey { get; set; }
    }
}