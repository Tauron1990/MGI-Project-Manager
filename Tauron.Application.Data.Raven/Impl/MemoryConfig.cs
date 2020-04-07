using System.Collections.Generic;

namespace Tauron.Application.Data.Raven.Impl
{
    public sealed class MemoryConfig
    {
        public Dictionary<string, InMemoryStore> MemoryStores { get; set; } = new Dictionary<string, InMemoryStore>();
    }
}