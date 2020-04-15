using System;
using System.Reflection;
using JetBrains.Annotations;

namespace Tauron.Application.Files.Serialization.Core.Fluent
{
    [PublicAPI]
    public interface IConstructorConfiguration<out TConfig>
    {
        TConfig Apply();

        IConstructorConfiguration<TConfig> WhithConstructor(ConstructorInfo info);

        IConstructorConfiguration<TConfig> WithCusomConstructor(Func<object?, object> constructor);

        IConstructorConfiguration<TConfig> WithObject(object parm);
    }
}