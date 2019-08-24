using System.Collections.Concurrent;
using CQRSlite.Snapshotting;
using Newtonsoft.Json.Linq;
using Tauron.CQRS.Common.Dto.Persistable;

namespace Tauron.CQRS.Services.Data
{
    public class AggregateStade : Snapshot, IObjectData
    {
        public ConcurrentDictionary<string, object> Objects { get; private set; } = new ConcurrentDictionary<string, object>();

        public JToken Create() => JToken.FromObject(Objects);

        public void Read(JToken token) => Objects = token.ToObject<ConcurrentDictionary<string, object>>();
    }
}