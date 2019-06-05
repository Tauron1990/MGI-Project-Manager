using System;
using System.Threading;
using System.Threading.Tasks;
using CQRSlite.Snapshotting;
using Microsoft.Extensions.Options;
using Tauron.CQRS.Common.Configuration;
using Tauron.CQRS.Common.Dto;
using Tauron.CQRS.Common.Dto.Persistable;

namespace Tauron.CQRS.Services.Core
{
    public class SnapshotServerStore : ISnapshotStore
    {
        private readonly IOptions<ClientCofiguration> _confOptions;
        private readonly IPersistApi _persistApi;

        public SnapshotServerStore(IOptions<ClientCofiguration> confOptions, IPersistApi persistApi)
        {
            _confOptions = confOptions;
            _persistApi = persistApi;
        }

        public async Task<Snapshot> Get(Guid id, CancellationToken cancellationToken = new CancellationToken()) 
            => (Snapshot)(await _persistApi.Get(new ApiObjectId {Id = id.ToString(), ApiKey = _confOptions.Value.ApiKey})).Data;

        public async Task Save(Snapshot snapshot, CancellationToken cancellationToken = new CancellationToken())
        {
            if (snapshot is IObjectData objectData)
            {
                await _persistApi.Put(new ApiObjectStade
                {
                    ApiKey = _confOptions.Value.ApiKey,
                    ObjectStade = new ObjectStade
                    {
                        Data = objectData,
                        Identifer = snapshot.Id.ToString()
                    }
                });
            }
            else
                throw new InvalidOperationException("IObjectData is needed for Snapshot");
        }
    }
}