using Tauron.CQRS.Common.Dto.Persistable;

namespace Tauron.CQRS.Common.Dto
{
    public class ApiObjectStade
    {
        public string? ApiKey { get; set; }

        public ObjectStade? ObjectStade { get; set; }
    }
}