using CQRSlite.Snapshotting;
using Newtonsoft.Json.Linq;

namespace Tauron.CQRS.Common.Dto.Persistable
{
    public class ObjectStade
    {
        public string? OriginalType { get; set; }

        public string? Identifer { get; set; }

        public JToken? Data { get; set; }
    }
}