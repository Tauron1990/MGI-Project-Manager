using System.Collections.Generic;
using Tauron.Application.Data.Raven.Impl;

namespace Tauron.Application.Data.Raven
{
    public sealed class DataRavenConfiguration
    {
        internal readonly Dictionary<string, InMemoryStore> MemoryStores = new Dictionary<string, InMemoryStore>();

        public DataRavenConfiguration AddMemoryStore(string name, InMemoryStore store)
        {
            MemoryStores[name] = store;
            return this;
        }
    }
}