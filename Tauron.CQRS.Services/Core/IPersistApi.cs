using System.Threading.Tasks;
using RestEase;
using Tauron.CQRS.Common.Dto;
using Tauron.CQRS.Common.Dto.Persistable;

namespace Tauron.CQRS.Services.Core
{
    public interface IPersistApi
    {
        [Get]
        Task<ObjectStade> Get([Body] ApiObjectId id);

        [Put]
        Task Put([Body] ApiObjectStade stade);
    }
}