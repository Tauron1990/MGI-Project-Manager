using JetBrains.Annotations;

namespace Tauron.Application.Files.Serialization.Core.Fluent
{
    [PublicAPI]
    public interface IXmlListElementConfiguration : IXmlRootConfiguration<IXmlListElementConfiguration>
    {
        [NotNull]
        IXmlListElementConfiguration Element([NotNull] string name);

        [NotNull]
        IXmlListAttributeConfiguration Attribute([NotNull] string name);

        [NotNull]
        IXmlSerializerConfiguration WithSubSerializer<TSerisalize>([NotNull] ISerializer serializer);
    }
}