using System.IO;
using JetBrains.Annotations;

namespace Akka.Code.Configuration.Serialization
{
    [PublicAPI]
    public sealed class ConfigSerializer
    {
        public AkkaRootConfiguration Read(string file)
        {
            using var stream = File.Open(file, FileMode.Open);
            return Read(stream);
        }

        public AkkaRootConfiguration Read(Stream stream)
        {
            using var reader = new BinaryReader(stream);
            return Read(reader);
        }

        public AkkaRootConfiguration Read(BinaryReader reader)
        {
            var config = new AkkaRootConfiguration();
            ((IBinarySerializable)config).Read(reader);
            return config;
        }

        public void Write(string file, AkkaRootConfiguration config)
        {
            using var stream = File.Open(file, FileMode.Create);
            Write(stream, config);
        }

        public void Write(Stream stream, AkkaRootConfiguration config)
        {
            using var writer = new BinaryWriter(stream);
            Write(writer, config);
        }

        public void Write(BinaryWriter writer, AkkaRootConfiguration config) 
            => ((IBinarySerializable)config).Write(writer);
    }
}