using System.Xml.Serialization;
using JetBrains.Annotations;

namespace Tauron.Application.Files.Serialization.Core.Fluent
{
    public interface IXmlElementConfiguration : IXmlRootConfiguration<IXmlElementConfiguration>
    {
        [NotNull]
        IXmlSerializerConfiguration WithSerializer([NotNull] XmlSerializer serializer);

        [NotNull]
        IXmlSerializerConfiguration WithSubSerializer<TTarget>([NotNull] ISerializer serializer);

        [NotNull]
        IXmlElementConfiguration Element([NotNull] string name);

        [NotNull]
        IXmlAttributConfiguration Attribute([NotNull] string name);

        [NotNull]
        IXmlListElementConfiguration WithElements([NotNull] string name);
    }
}