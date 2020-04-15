using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Tauron.Application.Files.Serialization.Core.Impl
{
    internal sealed class BinarySerializer : ISerializer
    {
        private readonly BinaryFormatter _formatter;

        public BinarySerializer(BinaryFormatter formatter)
        {
            _formatter = formatter;
        }

        public AggregateException? Errors => _formatter == null ? new AggregateException(new SerializerElementNullException("Formatter")) : null;

        public void Serialize(IStreamSource target, object graph)
        {
            Argument.NotNull(target, nameof(target));
            Argument.NotNull(graph, nameof(graph));

            using (var stream = target.OpenStream(FileAccess.ReadWrite))
            {
                _formatter.Serialize(stream, graph);
            }
        }

        public object Deserialize(IStreamSource target)
        {
            Argument.NotNull(target, nameof(target));

            using var stream = target.OpenStream(FileAccess.ReadWrite);
            return _formatter.Deserialize(stream);
        }

        public void Deserialize(IStreamSource targetStream, object target)
        {
            throw new NotSupportedException();
        }
    }
}