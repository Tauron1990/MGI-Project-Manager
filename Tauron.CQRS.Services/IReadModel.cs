using System.Threading.Tasks;
using CQRSlite.Queries;

namespace Tauron.CQRS.Services
{
    public interface IReadModel<TRespond>
    {
        Task<TRespond> ResolveQuery(IQuery<TRespond> query);
    }
}