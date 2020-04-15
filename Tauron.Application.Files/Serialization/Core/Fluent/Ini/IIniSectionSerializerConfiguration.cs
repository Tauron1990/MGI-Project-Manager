using JetBrains.Annotations;

namespace Tauron.Application.Files.Serialization.Core.Fluent
{
    [PublicAPI]
    public interface IIniSectionSerializerConfiguration
    {
        IIniKeySerializerConfiguration WithSingleKey();

        IIniKeySerializerConfiguration WithListKey();
    }
}