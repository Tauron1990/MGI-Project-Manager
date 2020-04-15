using System.Collections.Generic;

namespace Tauron.Application.Files.Serialization.Core.Managment
{
    public class SimpleMapper<TContext>
        where TContext : IOrginalContextProvider
    {
        public SimpleMapper()
        {
            Entries = new List<MappingEntry<TContext>>();
        }

        public List<MappingEntry<TContext>> Entries { get; }
    }
}