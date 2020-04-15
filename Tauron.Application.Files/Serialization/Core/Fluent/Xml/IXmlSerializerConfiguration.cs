using JetBrains.Annotations;

namespace Tauron.Application.Files.Serialization.Core.Fluent
{
    public interface IXmlSerializerConfiguration : ISerializerRootConfiguration, IConstructorConfig<IXmlSerializerConfiguration>
    {
        [NotNull]
        IXmlAttributConfiguration WithAttribut([NotNull] string name);

        [NotNull]
        IXmlElementConfiguration WithElement([NotNull] string name);

        [NotNull]
        IXmlListElementConfiguration WithElements([NotNull] string name);

        [NotNull]
        IXmlSerializerConfiguration WithSubSerializer<TTarget>([NotNull] ISerializer serializer, [NotNull] string member);
    }
}