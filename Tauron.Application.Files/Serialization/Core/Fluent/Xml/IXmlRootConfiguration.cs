using System.Xml.Linq;

namespace Tauron.Application.Files.Serialization.Core.Fluent
{
    public interface IXmlRootConfiguration<out TConfig> : IWithMember<TConfig>
    {
        IXmlSerializerConfiguration Apply();

        TConfig WithNamespace(XNamespace xNamespace);
    }
}