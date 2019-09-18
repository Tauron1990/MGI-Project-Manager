using System;
using CQRSlite.Events;
using Newtonsoft.Json.Linq;

namespace Tauron.CQRS.Services.Core
{
    public class QueryResultMessage : IEvent
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public int Version { get; set; } = -1;
        public DateTimeOffset TimeStamp { get; set; } = DateTimeOffset.Now;

        public string TypeName { get; set; }

        public JToken Data { get; set; }
    }
}