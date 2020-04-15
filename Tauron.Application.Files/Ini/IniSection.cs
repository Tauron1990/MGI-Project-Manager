using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Tauron.Application.Files.Ini
{
    [PublicAPI]
    [Serializable]
    public sealed class IniSection
    {
        private readonly Dictionary<string, IniEntry> _entries;

        public IniSection(Dictionary<string, IniEntry> entries, string name)
        {
            Name = Argument.NotNull(name, nameof(name));
            _entries = Argument.NotNull(entries, nameof(entries));
        }

        public IniSection(string name)
            : this(new Dictionary<string, IniEntry>(), name)
        {
        }

        public ReadOnlyEnumerator<IniEntry> Entries => new ReadOnlyEnumerator<IniEntry>(_entries.Values);

        public string Name { get; private set; }

        public SingleIniEntry? GetSingleEntry(string name)
        {
            if (_entries.TryGetValue(name, out var entry))
                return entry as SingleIniEntry;
            return null;
        }

        public SingleIniEntry AddSingleKey(string name)
        {
            var entry = new SingleIniEntry(name, null);
            _entries[name] = entry;
            return entry;
        }

        public ListIniEntry? GetListEntry(string name)
        {
            if (!_entries.TryGetValue(name, out var value)) return null;

            if (value is ListIniEntry multi) return multi;

            multi = new ListIniEntry((SingleIniEntry) value);
            _entries[multi.Key] = multi;

            return multi;
        }

        public ListIniEntry GetOrAddListEntry(string name)
        {
            var entry = GetListEntry(name);
            if (entry != null)
                return entry;

            entry = new ListIniEntry(name, new List<string>());

            return entry;
        }

        public SingleIniEntry GetData(string name)
        {
            var data = GetSingleEntry(name);

            return data ?? AddSingleKey(name);
        }
    }
}