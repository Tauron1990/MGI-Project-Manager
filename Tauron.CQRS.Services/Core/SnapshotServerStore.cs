using System;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using CQRSlite.Snapshotting;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Tauron.CQRS.Common.Configuration;
using Tauron.CQRS.Common.Dto;
using Tauron.CQRS.Common.Dto.Persistable;

namespace Tauron.CQRS.Services.Core
{
    public class SnapshotServerStore : ISnapshotStore
    {
        private static class SerializerFactory
        {
            private class TauronBinder : ISerializationBinder
            {
                public static readonly TauronBinder Binder = new TauronBinder();

                private readonly string[] _safeNameSpaces = {"System", "System.Collections", "System.Collections.Generic", "System.Collections.Concurrent"};

                public Type BindToType(string assemblyName, string typeName)
                {
                    ChackType(typeName);
                    return Type.GetType($"{typeName}, {assemblyName}");
                }

                public void BindToName(Type serializedType, out string assemblyName, out string typeName)
                {
                    typeName = serializedType.FullName;
                    ChackType(typeName);
                    assemblyName = serializedType.Assembly.FullName;
                }

                private void ChackType(string typeName)
                {
                    var nameSpace = typeName[..typeName.LastIndexOf('.')];

                    if(_safeNameSpaces.Contains(nameSpace)) return;
                    if(nameSpace.StartsWith("Tauron")) return;

                    throw new InvalidOperationException($"Serializer Forbidden Type: {typeName}");
                }
            }

            public static JsonSerializer Create()
            {
                return JsonSerializer.Create(new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All,
                    SerializationBinder = TauronBinder.Binder
                });
            }
        }

        private readonly IOptions<ClientCofiguration> _confOptions;
        private readonly IPersistApi _persistApi;

        public SnapshotServerStore(IOptions<ClientCofiguration> confOptions, IPersistApi persistApi)
        {
            _confOptions = confOptions;
            _persistApi = persistApi;
        }

        public async Task<Snapshot> Get(Guid id, CancellationToken cancellationToken = new CancellationToken())
        {
            var stade = await _persistApi.Get(new ApiObjectId {Id = id.ToString(), ApiKey = _confOptions.Value.ApiKey});
            if (stade?.OriginalType == null) return null;

            return (Snapshot) stade.Data?.ToObject(Type.GetType(stade.OriginalType), SerializerFactory.Create());
        }

        public async Task Save(Snapshot snapshot, CancellationToken cancellationToken = new CancellationToken())
        {
            if (snapshot is IObjectData objectData)
            {
                await _persistApi.Put(new ApiObjectStade
                {
                    ApiKey = _confOptions.Value.ApiKey,
                    ObjectStade = new ObjectStade
                    {
                        Data = JToken.FromObject(objectData, SerializerFactory.Create()),
                        Identifer = snapshot.Id.ToString(),
                        OriginalType = objectData.GetType().AssemblyQualifiedName
                    }
                });
            }
            else
                throw new InvalidOperationException("IObjectData is needed for Snapshot");
        }
    }
}