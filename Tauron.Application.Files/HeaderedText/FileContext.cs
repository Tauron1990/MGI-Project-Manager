using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace Tauron.Application.Files.HeaderedText
{
    [PublicAPI]
    public sealed class FileContext : IEnumerable<ContextEntry>
    {
        internal FileContext([NotNull] FileDescription description) 
            => Description = (FileDescription) description.Clone();

        internal FileDescription Description { get; }

        internal List<ContextEntry> ContextEnries { get; } = new List<ContextEntry>();

        public IEnumerable<ContextEntry> this[string key] 
            => ContextEnries.Where(contextEnry => contextEnry.Key == key);

        public IEnumerator<ContextEntry> GetEnumerator() 
            => ContextEnries.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() 
            => GetEnumerator();

        internal void Reset() 
            => ContextEnries.Clear();

        internal bool IsKeyword([NotNull] string key) 
            => Description.Contains(key);

        internal void Add([NotNull] ContextEntry entry) 
            => ContextEnries.Add(entry);
    }
}