using Newtonsoft.Json.Linq;

namespace Tauron.CQRS.Common.Dto.Persistable
{
    public interface IJsonFormatable
    {
        JToken Create();

        void Read(JToken token);
    }
}