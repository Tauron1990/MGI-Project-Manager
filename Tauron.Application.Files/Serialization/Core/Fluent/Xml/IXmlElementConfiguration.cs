using System.Xml.Serialization;
using JetBrains.Annotations;

namespace Tauron.Application.Files.Serialization.Core.Fluent
{
    [PublicAPI]
    public interface IXmlElementConfiguration : IXmlRootConfiguration<IXmlElementConfiguration>
    {
        IXmlSerializerConfiguration WithSerializer(XmlSerializer serializer);

        IXmlSerializerConfiguration WithSubSerializer<TTarget>(ISerializer serializer);

        IXmlElementConfiguration Element(string name);

        IXmlAttributConfiguration Attribute(string name);

        IXmlListElementConfiguration WithElements(string name);
    }
}