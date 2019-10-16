using System.Threading.Tasks;
using CQRSlite.Queries;
using Tauron.CQRS.Common.ServerHubs;

namespace Tauron.CQRS.Services.Core
{
    public interface IReadModel<TRespond, in TQuery>
        where TQuery : IQuery<TRespond>
    {
        Task ResolveQuery(TQuery query, ServerDomainMessage serverDomainMessage);
    }
}