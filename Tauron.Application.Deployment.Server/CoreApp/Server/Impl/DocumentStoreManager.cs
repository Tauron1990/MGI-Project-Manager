using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Raven.Client.Documents;

namespace Tauron.Application.Deployment.Server.CoreApp.Server.Impl
{
    public sealed class DocumentStoreManager : IDocumentStoreManager
    {
        private readonly CoreConfig _coreConfig;
        private readonly ConcurrentDictionary<string, IDocumentStore> _documentStores = new ConcurrentDictionary<string, IDocumentStore>();


        public DocumentStoreManager(CoreConfig coreConfig)
        {
            _coreConfig = coreConfig;
        }

        public void Dispose()
        {
            foreach (var documentStore in _documentStores.Values) 
                documentStore.Dispose();
        }

        public IDocumentStore Get(string name) 
            => _documentStores.GetOrAdd(name, s => new DocumentStore {Database = name, Urls = new[] {_coreConfig.ConnectionString}});
    }
}
