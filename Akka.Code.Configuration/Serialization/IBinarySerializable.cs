using System.IO;

namespace Akka.Code.Configuration.Serialization
{
    public interface IBinarySerializable
    {
        void Write(BinaryWriter writer);
        void Read(BinaryReader reader);
    }
}