using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using JetBrains.Annotations;

namespace Tauron
{
    [PublicAPI]
    public static class SerializationExtensions
    {
        private class XmlSerilalizerDelegator : IFormatter
        {
            private readonly XmlSerializer _serializer;

            public XmlSerilalizerDelegator([NotNull] XmlSerializer serializer)
            {
                Argument.NotNull(serializer, nameof(serializer));
                _serializer = serializer;
            }

            public SerializationBinder? Binder
            {
                get => null;

                set { }
            }

            public StreamingContext Context
            {
                get => new StreamingContext();

                set { }
            }

            public ISurrogateSelector? SurrogateSelector
            {
                get => null;

                set { }
            }
            
            public object Deserialize(Stream serializationStream) => _serializer.Deserialize(serializationStream);

            public void Serialize(Stream serializationStream, object graph) => _serializer.Serialize(serializationStream, graph);
        }

        public static TValue Deserialize<TValue>(this string path, IFormatter formatter)
            where TValue : class
        {
            Argument.NotNull(path, nameof(path));
            Argument.NotNull(formatter, nameof(formatter));
            using var stream = path.OpenRead();
            return (TValue) InternalDeserialize(formatter, stream);
        }

        public static TValue Deserialize<TValue>(this string path)
            where TValue : class
        {
            Argument.NotNull(path, nameof(path));
            if (!path.ExisFile()) return Activator.CreateInstance<TValue>();

            using (var stream = path.OpenRead())
                return (TValue) InternalDeserialize(new BinaryFormatter(), stream);
        }

        public static void Serialize([NotNull] this object graph, IFormatter formatter, string path)
        {
            Argument.NotNull(graph, nameof(graph));
            Argument.NotNull(formatter, nameof(formatter));
            Argument.NotNull(path, path);

            using var stream = path.OpenWrite();
            InternalSerialize(graph, formatter, stream);
        }

        public static void Serialize(this object graph, string path)
        {
            Argument.NotNull(graph, nameof(path));
            Argument.NotNull(path, nameof(path));
            path.CreateDirectoryIfNotExis();

            using var stream = path.OpenWrite();
            InternalSerialize(graph, new BinaryFormatter(), stream);
        }

        public static TValue XmlDeserialize<TValue>(this string path, XmlSerializer formatter)
            where TValue : class
        {
            Argument.NotNull(path, nameof(path));
            Argument.NotNull(formatter, nameof(formatter));

            using var stream = path.OpenRead();
            return (TValue) InternalDeserialize(new XmlSerilalizerDelegator(formatter), stream);
        }

        public static TValue XmlDeserializeIfExis<TValue>(this string path, XmlSerializer formatter)
            where TValue : class
        {
            Argument.NotNull(path, nameof(path));
            Argument.NotNull(formatter, nameof(formatter));

            return path.ExisFile() ? XmlDeserialize<TValue>(path, formatter) : Activator.CreateInstance<TValue>();
        }

        public static void XmlSerialize(this object graph, XmlSerializer formatter, string path)
        {
            Argument.NotNull(graph, nameof(graph));
            Argument.NotNull(formatter, nameof(formatter));
            Argument.NotNull(path, nameof(path));

            using var stream = path.OpenWrite();
            InternalSerialize(graph, new XmlSerilalizerDelegator(formatter), stream);
        }

        
        private static object InternalDeserialize(IFormatter formatter, Stream stream) => formatter.Deserialize(stream);

        private static void InternalSerialize(object graph, IFormatter formatter, Stream stream) => formatter.Serialize(stream, graph);
    }
}