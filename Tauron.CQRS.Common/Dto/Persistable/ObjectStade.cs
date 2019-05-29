using Newtonsoft.Json;
using Tauron.CQRS.Common.Dto.TypeHandling;

namespace Tauron.CQRS.Common.Dto.Persistable
{
    public class ObjectStade
    {
        public string Identifer { get; set; }

        [JsonConverter(typeof(TypeResolver))]
        public IObjectData Data { get; set; }
    }
}