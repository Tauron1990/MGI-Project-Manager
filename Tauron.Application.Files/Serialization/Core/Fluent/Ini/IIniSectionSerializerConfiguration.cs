using JetBrains.Annotations;

namespace Tauron.Application.Files.Serialization.Core.Fluent
{
    public interface IIniSectionSerializerConfiguration
    {
        [NotNull]
        IIniKeySerializerConfiguration WithSingleKey();

        [NotNull]
        IIniKeySerializerConfiguration WithListKey();
    }
}