using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using Tauron.Application.Files.Serialization.Core.Impl;

namespace Tauron.Application.Files.Serialization.Core.Fluent.Impl
{
    internal class BinarySerializerConfiguration : SerializerRootConfigurationBase, IBinaryConfiguration
    {
        private readonly BinaryFormatter _binaryFormatter = new BinaryFormatter();

        public IBinaryConfiguration WithFormat(FormatterAssemblyStyle format)
        {
            _binaryFormatter.AssemblyFormat = format;
            return this;
        }

        public IBinaryConfiguration WithBinder(SerializationBinder binder)
        {
            _binaryFormatter.Binder = binder;
            return this;
        }

        public IBinaryConfiguration WithContext(StreamingContext context)
        {
            _binaryFormatter.Context = context;
            return this;
        }

        public IBinaryConfiguration WithFilterLevel(TypeFilterLevel level)
        {
            _binaryFormatter.FilterLevel = level;
            return this;
        }

        public IBinaryConfiguration WithSelector(ISurrogateSelector selector)
        {
            _binaryFormatter.SurrogateSelector = selector;
            return this;
        }

        public IBinaryConfiguration WithFormat(FormatterTypeStyle format)
        {
            _binaryFormatter.TypeFormat = format;
            return this;
        }

        public override ISerializer ApplyInternal()
        {
            var serializer = new BinarySerializer(_binaryFormatter);

            VerifyErrors(serializer);

            return serializer;
        }
    }
}