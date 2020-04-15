using JetBrains.Annotations;

namespace Tauron.Application.Files.Serialization.Core.Fluent
{
    [PublicAPI]
    public interface IHeaderedFileSerializerConfiguration : ISerializerRootConfiguration, IConstructorConfig<IHeaderedFileSerializerConfiguration>
    {
        [NotNull]
        IHeaderedFileKeywordConfiguration AddKeyword([NotNull] string key);

        [NotNull]
        IHeaderedFileKeywordConfiguration AddKeywordList([NotNull] string key);

        [NotNull]
        IHeaderedFileKeywordConfiguration MapContent();
    }
}