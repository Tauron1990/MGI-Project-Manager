using System.Collections.Generic;
using CQRSlite.Snapshotting;
using Newtonsoft.Json.Linq;
using Tauron.CQRS.Common.Dto.Persistable;

namespace Tauron.CQRS.Services.Data
{
    public sealed class AggregateData : Snapshot, IObjectData
    {
        public Dictionary<string, object> Objects { get; set; }

        public JToken Create() => JToken.FromObject(Objects);

        public void Read(JToken token) => Objects = token.ToObject<Dictionary<string, object>>();
    }
}