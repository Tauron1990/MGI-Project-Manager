namespace Tauron.Application.Files.Serialization.Core.Fluent
{
    public interface IConstructorConfig<out TConfig>
    {
        IConstructorConfiguration<TConfig> ConfigConstructor();
    }
}