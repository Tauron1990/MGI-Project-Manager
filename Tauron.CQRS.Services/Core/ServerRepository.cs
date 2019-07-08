using System;
using System.Threading;
using System.Threading.Tasks;
using CQRSlite.Domain;
using Microsoft.Extensions.Options;
using Tauron.CQRS.Common.Configuration;
using Tauron.CQRS.Common.Dto;

namespace Tauron.CQRS.Services.Core
{
    //public class ServerRepository : IRepository
    //{
    //    private const string AggregatePrefix = "Aggregate_";

    //    private readonly IPersistApi _persistApi;
    //    private readonly IOptions<ClientCofiguration> _config;

    //    public ServerRepository(IPersistApi persistApi, IOptions<ClientCofiguration> config)
    //    {
    //        _persistApi = persistApi;
    //        _config = config;
    //    }

    //    public async Task Save<T>(T aggregate, int? expectedVersion = null, CancellationToken cancellationToken = new CancellationToken()) where T : AggregateRoot
    //    {
    //        if (aggregate is CoreAggregateRoot coreAggregateRoot)
    //        {
    //            await _persistApi.Put(new ApiObjectStade
    //            {
    //                ApiKey = _config.Value.ApiKey,
    //                ObjectStade = 
    //            });

    //            return;
    //        }

    //        Throw();
    //    }

    //    public Task<T> Get<T>(Guid aggregateId, CancellationToken cancellationToken = new CancellationToken()) where T : AggregateRoot
    //    {

    //    }

    //    private void Throw() => throw new InvalidOperationException("CoreAggregateRoot BaseType Requietment");
    //}
}