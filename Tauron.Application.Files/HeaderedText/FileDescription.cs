using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Tauron.Application.Files.HeaderedText
{
    [PublicAPI]
    public class FileDescription : ICloneable
    {
        private readonly HashSet<string> _keys;

        private IEnumerable<string>? _readonlyEnumerable;

        public FileDescription() 
            => _keys = new HashSet<string>();

        private FileDescription(FileDescription parent) 
            => _keys = new HashSet<string>(Argument.NotNull(parent, nameof(parent)).Keys);

        public IEnumerable<string> Keys => _readonlyEnumerable ??= new ReadOnlyEnumerator<string>(_keys);

        public object Clone() 
            => new FileDescription(this);

        public bool Add(string key) 
            => _keys.Add(key);

        public bool Remove(string key) 
            => _keys.Remove(key);

        public bool Contains(string key) 
            => _keys.Contains(key);
    }
}