using JetBrains.Annotations;

namespace Tauron.Application.Files.Serialization.Core.Fluent
{
    public interface IConstructorConfig<out TConfig>
    {
        [NotNull]
        IConstructorConfiguration<TConfig> ConfigConstructor();
    }
}