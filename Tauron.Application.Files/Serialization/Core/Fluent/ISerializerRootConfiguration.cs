using JetBrains.Annotations;

namespace Tauron.Application.Files.Serialization.Core.Fluent
{
    [PublicAPI]
    public interface ISerializerRootConfiguration
    {
        TypedSerializer<TType> Apply<TType>()
            where TType : class;
    }
}