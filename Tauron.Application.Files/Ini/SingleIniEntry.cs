using System;
using JetBrains.Annotations;

namespace Tauron.Application.Files.Ini
{
    [PublicAPI]
    [Serializable]
    public sealed class SingleIniEntry : IniEntry
    {
        internal SingleIniEntry(string key, string? value)
            : base(key)
        {
            Value = value ?? string.Empty;
        }

        public string Value { get; set; }
    }
}