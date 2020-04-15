using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace Tauron.Application.Files.HeaderedText
{
    [PublicAPI]
    public sealed class FileContext : IEnumerable<ContextEnry>
    {
        internal FileContext([NotNull] FileDescription description) => Description = (FileDescription) description.Clone();

        [NotNull]
        internal FileDescription Description { get; }

        [NotNull]
        internal List<ContextEnry> ContextEnries { get; } = new List<ContextEnry>();

        public IEnumerable<ContextEnry> this[string key] => ContextEnries.Where(contextEnry => contextEnry.Key == key);

        public IEnumerator<ContextEnry> GetEnumerator() => ContextEnries.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        internal void Reset() => ContextEnries.Clear();

        internal bool IsKeyword([NotNull] string key) => Description.Contains(key);

        internal void Add([NotNull] ContextEnry entry) => ContextEnries.Add(entry);
    }
}