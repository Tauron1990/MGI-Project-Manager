using System;
using System.Collections.Concurrent;

namespace Tauron.CQRS.Common.Dto.TypeHandling.Impl
{
    public class TypeRegistry : ITypeRegistry
    {
        private readonly ConcurrentDictionary<string, Type> _types = new ConcurrentDictionary<string, Type>();

        public void Register(string name, Type namedType) => _types[name] = namedType;

        public Type Resolve(string name) => _types.TryGetValue(name, out var type) ? type : null;
    }
}