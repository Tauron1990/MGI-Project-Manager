using System;
using JetBrains.Annotations;
using Tauron.CQRS.Common.Dto.TypeHandling;

namespace Tauron.CQRS.Common.Configuration
{
    [PublicAPI]
    public class CommonConfiguration
    {
        private readonly ITypeRegistry _registry = TypeResolver.TypeRegistry;
        public void RegisterType(string name, Type namedType) => _registry.Register(name, namedType);


    }
}