using System.Threading.Tasks;
using CQRSlite.Queries;

namespace Tauron.CQRS.Services.Core
{
    public interface IReadModel<TRespond, TQuery>
        where TQuery : IQuery<TRespond>
    {
        Task ResolveQuery(TQuery query);
    }
}