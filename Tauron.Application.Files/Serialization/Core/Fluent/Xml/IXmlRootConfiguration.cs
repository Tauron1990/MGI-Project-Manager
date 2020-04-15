using System.Xml.Linq;
using JetBrains.Annotations;

namespace Tauron.Application.Files.Serialization.Core.Fluent
{
    public interface IXmlRootConfiguration<out TConfig> : IWithMember<TConfig>
    {
        [NotNull]
        IXmlSerializerConfiguration Apply();

        [NotNull]
        TConfig WithNamespace([NotNull] XNamespace xNamespace);
    }
}