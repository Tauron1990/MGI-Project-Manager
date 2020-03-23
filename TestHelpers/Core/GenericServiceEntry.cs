using System;
using Microsoft.Extensions.DependencyInjection;

namespace TestHelpers.Core
{
    public sealed class GenericServiceEntry<TInterface, TType> : ServiceEntry
        where TType : TInterface where TInterface : class
    {
        public GenericServiceEntry(TType service) 
            => Service = service;

        public TType Service { get; }

        public Action<TType>? Asseration { get; set; }

        public override void Register(IServiceCollection collection) 
            => collection.AddSingleton<TInterface>(Service);

        public override void Assert() 
            => Asseration?.Invoke(Service);
    }
}