using System.Threading.Tasks;
using Tauron.CQRS.Common.Dto.Persistable;

namespace Tauron.CQRS.Services.Core
{
    public interface IPersistApi
    {
        Task<ObjectStade> Get(string id);

        Task Put(ObjectStade stade);
    }
}