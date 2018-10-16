#region

using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using JetBrains.Annotations;

#endregion

namespace Tauron
{
    [PublicAPI]
    public static class SerializationExtensions
    {
        private class XmlSerilalizerDelegator : IFormatter
        {
            #region Fields

            /// <summary>The _serializer.</summary>
            private readonly XmlSerializer _serializer;

            #endregion

            #region Constructors and Destructors

            /// <summary>
            ///     Initializes a new instance of the <see cref="XmlSerilalizerDelegator" /> class.
            ///     Initialisiert eine neue Instanz der <see cref="XmlSerilalizerDelegator" /> Klasse.
            ///     Initializes a new instance of the <see cref="XmlSerilalizerDelegator" /> class.
            /// </summary>
            /// <param name="serializer">
            ///     The serializer.
            /// </param>
            public XmlSerilalizerDelegator([NotNull] XmlSerializer serializer)
            {
                if (serializer == null) throw new ArgumentNullException(nameof(serializer));
                _serializer = serializer;
            }

            #endregion

            #region Public Properties

            /// <summary>Gets or sets the binder.</summary>
            /// <value>The binder.</value>
            [CanBeNull]
            public SerializationBinder Binder
            {
                get => null;

                set { }
            }

            /// <summary>Gets or sets the context.</summary>
            /// <value>The context.</value>
            public StreamingContext Context
            {
                get => new StreamingContext();

                set { }
            }

            /// <summary>Gets or sets the surrogate selector.</summary>
            /// <value>The surrogate selector.</value>
            [CanBeNull]
            public ISurrogateSelector SurrogateSelector
            {
                get => null;

                set { }
            }

            #endregion

            #region Public Methods and Operators

            /// <summary>
            ///     The deserialize.
            /// </summary>
            /// <param name="serializationStream">
            ///     The serialization stream.
            /// </param>
            /// <returns>
            ///     The <see cref="object" />.
            /// </returns>
            [NotNull]
            public object Deserialize([NotNull] Stream serializationStream)
            {
                return _serializer.Deserialize(serializationStream);
            }

            /// <summary>
            ///     The serialize.
            /// </summary>
            /// <param name="serializationStream">
            ///     The serialization stream.
            /// </param>
            /// <param name="graph">
            ///     The graph.
            /// </param>
            public void Serialize([NotNull] Stream serializationStream, [NotNull] object graph)
            {
                _serializer.Serialize(serializationStream, graph);
            }

            #endregion
        }

        #region Public Methods and Operators

        /// <summary>
        ///     The deserialize.
        /// </summary>
        /// <param name="path">
        ///     The path.
        /// </param>
        /// <param name="formatter">
        ///     The formatter.
        /// </param>
        /// <typeparam name="TValue">
        /// </typeparam>
        /// <returns>
        ///     The <see cref="TValue" />.
        /// </returns>
        [NotNull]
        public static TValue Deserialize<TValue>([NotNull] this string path, [NotNull] IFormatter formatter)
            where TValue : class
        {
            if (path == null) throw new ArgumentNullException(nameof(path));
            if (formatter == null) throw new ArgumentNullException(nameof(formatter));
            using (var stream = File.OpenRead(path))
            {
                return (TValue) InternalDeserialize(formatter, stream);
            }
        }

        /// <summary>
        ///     The deserialize.
        /// </summary>
        /// <param name="path">
        ///     The path.
        /// </param>
        /// <typeparam name="TValue">
        /// </typeparam>
        /// <returns>
        ///     The <see cref="TValue" />.
        /// </returns>
        [NotNull]
        public static TValue Deserialize<TValue>([NotNull] this string path)
            where TValue : class
        {
            if (path == null) throw new ArgumentNullException(nameof(path));
            if (!File.Exists(path)) return Activator.CreateInstance<TValue>();

            using (var stream = File.OpenRead(path))
            {
                return (TValue) InternalDeserialize(new BinaryFormatter(), stream);
            }
        }

        /// <summary>
        ///     The serialize.
        /// </summary>
        /// <param name="graph">
        ///     The graph.
        /// </param>
        /// <param name="formatter">
        ///     The formatter.
        /// </param>
        /// <param name="path">
        ///     The path.
        /// </param>
        public static void Serialize([NotNull] this object graph, [NotNull] IFormatter formatter, [NotNull] string path)
        {
            if (graph == null) throw new ArgumentNullException(nameof(graph));
            if (formatter == null) throw new ArgumentNullException(nameof(formatter));
            if (path == null) throw new ArgumentNullException(nameof(path));
            using (var stream = File.OpenWrite(path))
            {
                InternalSerialize(graph, formatter, stream);
            }
        }

        /// <summary>
        ///     The serialize.
        /// </summary>
        /// <param name="graph">
        ///     The graph.
        /// </param>
        /// <param name="path">
        ///     The path.
        /// </param>
        public static void Serialize([NotNull] this object graph, [NotNull] string path)
        {
            if (graph == null) throw new ArgumentNullException(nameof(graph));
            if (path == null) throw new ArgumentNullException(nameof(path));
            path.CreateDirectoryIfNotExis();
            using (var stream = File.OpenWrite(path))
            {
                InternalSerialize(graph, new BinaryFormatter(), stream);
            }
        }

        /// <summary>
        ///     The xml deserialize.
        /// </summary>
        /// <param name="path">
        ///     The path.
        /// </param>
        /// <param name="formatter">
        ///     The formatter.
        /// </param>
        /// <typeparam name="TValue">
        /// </typeparam>
        /// <returns>
        ///     The <see cref="TValue" />.
        /// </returns>
        [NotNull]
        public static TValue XmlDeserialize<TValue>([NotNull] this string path, [NotNull] XmlSerializer formatter)
            where TValue : class
        {
            if (path == null) throw new ArgumentNullException(nameof(path));
            if (formatter == null) throw new ArgumentNullException(nameof(formatter));
            using (var stream = File.OpenRead(path))
            {
                return (TValue) InternalDeserialize(new XmlSerilalizerDelegator(formatter), stream);
            }
        }

        /// <summary>
        ///     The xml deserialize if exis.
        /// </summary>
        /// <param name="path">
        ///     The path.
        /// </param>
        /// <param name="formatter">
        ///     The formatter.
        /// </param>
        /// <typeparam name="TValue">
        /// </typeparam>
        /// <returns>
        ///     The <see cref="TValue" />.
        /// </returns>
        [NotNull]
        public static TValue XmlDeserializeIfExis<TValue>([NotNull] this string path, [NotNull] XmlSerializer formatter)
            where TValue : class
        {
            if (path == null) throw new ArgumentNullException(nameof(path));
            if (formatter == null) throw new ArgumentNullException(nameof(formatter));
            if (string.IsNullOrEmpty(path)) throw new ArgumentException("Value cannot be null or empty.", nameof(path));

            return path.ExisFile() ? XmlDeserialize<TValue>(path, formatter) : Activator.CreateInstance<TValue>();
        }

        /// <summary>
        ///     The xml serialize.
        /// </summary>
        /// <param name="graph">
        ///     The graph.
        /// </param>
        /// <param name="formatter">
        ///     The formatter.
        /// </param>
        /// <param name="path">
        ///     The path.
        /// </param>
        public static void XmlSerialize([NotNull] this object graph, [NotNull] XmlSerializer formatter,
            [NotNull] string path)
        {
            if (graph == null) throw new ArgumentNullException(nameof(graph));
            if (formatter == null) throw new ArgumentNullException(nameof(formatter));
            if (path == null) throw new ArgumentNullException(nameof(path));
            using (var stream = File.OpenWrite(path))
            {
                InternalSerialize(graph, new XmlSerilalizerDelegator(formatter), stream);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        ///     The internal deserialize.
        /// </summary>
        /// <param name="formatter">
        ///     The formatter.
        /// </param>
        /// <param name="stream">
        ///     The stream.
        /// </param>
        /// <returns>
        ///     The <see cref="object" />.
        /// </returns>
        [NotNull]
        private static object InternalDeserialize([NotNull] IFormatter formatter, [NotNull] Stream stream)
        {
            if (formatter == null) throw new ArgumentNullException(nameof(formatter));
            if (stream == null) throw new ArgumentNullException(nameof(stream));
            return formatter.Deserialize(stream);
        }

        /// <summary>
        ///     The internal serialize.
        /// </summary>
        /// <param name="graph">
        ///     The graph.
        /// </param>
        /// <param name="formatter">
        ///     The formatter.
        /// </param>
        /// <param name="stream">
        ///     The stream.
        /// </param>
        private static void InternalSerialize([NotNull] object graph, [NotNull] IFormatter formatter,
            [NotNull] Stream stream)
        {
            if (graph == null) throw new ArgumentNullException(nameof(graph));
            if (formatter == null) throw new ArgumentNullException(nameof(formatter));
            if (stream == null) throw new ArgumentNullException(nameof(stream));
            formatter.Serialize(stream, graph);
        }

        #endregion
    }
}