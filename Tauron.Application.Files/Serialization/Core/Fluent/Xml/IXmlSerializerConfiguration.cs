namespace Tauron.Application.Files.Serialization.Core.Fluent
{
    public interface IXmlSerializerConfiguration : ISerializerRootConfiguration, IConstructorConfig<IXmlSerializerConfiguration>
    {
        IXmlAttributConfiguration WithAttribut(string name);

        IXmlElementConfiguration WithElement(string name);

        IXmlListElementConfiguration WithElements(string name);

        IXmlSerializerConfiguration WithSubSerializer<TTarget>(ISerializer serializer, string member);
    }
}