using Tauron.Application.Files.Serialization.Core;

namespace Tauron.Application.Files.Serialization
{
    public interface ISubSerializer : ISerializer
    {
        void Serialize(SerializationContext target, object graph);

        object Deserialize(SerializationContext target);
    }
}