using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Tauron.Application.Files.Ini
{
    [Serializable]
    public sealed class ListIniEntry : IniEntry
    {
        internal ListIniEntry(string key, List<string> values)
            : base(key)
        {
            Values = Argument.NotNull(values, nameof(values));
        }

        internal ListIniEntry(SingleIniEntry entry)
            : base(entry.Key)
        {
            Values = new List<string>(1) {entry.Value};
        }

        public List<string> Values { get; }
    }
}