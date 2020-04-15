using JetBrains.Annotations;

namespace Tauron.Application.Files.Serialization.Core.Fluent
{
    [PublicAPI]
    public interface IHeaderedFileKeywordConfiguration : IWithMember<IHeaderedFileKeywordConfiguration>
    {
        IHeaderedFileSerializerConfiguration Apply();
    }
}