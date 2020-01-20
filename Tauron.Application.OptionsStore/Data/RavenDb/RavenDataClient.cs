using System;
using System.Threading;
using Raven.Client.Documents;

namespace Tauron.Application.OptionsStore.Data.RavenDb
{
    public sealed class RavenDataClient : IDataClient
    {
        private readonly Lazy<IDocumentStore> _documetStore;

        public RavenDataClient(Func<IDocumentStore> documetStore) 
            => _documetStore = new Lazy<IDocumentStore>(documetStore, LazyThreadSafetyMode.ExecutionAndPublication);

        public IOptionDataCollection GetCollection(string name)
        {
            
        }
    }
}