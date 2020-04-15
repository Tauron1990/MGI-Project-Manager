using System;

namespace Tauron.Application.Files.Ini
{
    [Serializable]
    public abstract class IniEntry
    {
        protected IniEntry(string key)
        {
            Key = Argument.NotNull(key, nameof(key));
        }

        public string Key { get; }
    }
}