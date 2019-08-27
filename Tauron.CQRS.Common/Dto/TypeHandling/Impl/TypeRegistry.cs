using System;
using System.Collections.Concurrent;
using System.Linq;

namespace Tauron.CQRS.Common.Dto.TypeHandling.Impl
{
    //public class TypeRegistry : ITypeRegistry
    //{
    //    private readonly ConcurrentDictionary<string, Type> _types = new ConcurrentDictionary<string, Type>();

    //    public string GetName(Type type)
    //    {
    //        var temp = _types.FirstOrDefault(t => t.Value == type);

    //        return temp.Key;
    //    }

    //    public bool Contains(Type type) => _types.Any(p => p.Value == type);

    //    public void Register(string name, Type namedType) => _types[name] = namedType;

    //    public Type Resolve(string name) => _types.TryGetValue(name, out var type) ? type : null;
    //}
}