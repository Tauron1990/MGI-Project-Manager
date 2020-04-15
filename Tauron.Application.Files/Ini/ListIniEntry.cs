using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Tauron.Application.Files.Ini
{
    [Serializable]
    public sealed class ListIniEntry : IniEntry
    {
        internal ListIniEntry([NotNull] string key, [NotNull] List<string> values)
            : base(key) => Values = Argument.NotNull(values, nameof(values));

        internal ListIniEntry([NotNull] SingleIniEntry entry)
            : base(entry.Key) => Values = new List<string>(1) {entry.Value};

        [NotNull]
        public List<string> Values { get; }
    }
}