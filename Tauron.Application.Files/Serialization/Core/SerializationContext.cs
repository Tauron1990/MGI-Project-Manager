using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using JetBrains.Annotations;
using Tauron.Application.Files.Serialization.Sources;

namespace Tauron.Application.Files.Serialization.Core
{
    [PublicAPI]
    public sealed class SerializationContext : IDisposable
    {
        private BackgroundStream _backgroundStream;

        private Dictionary<string, SerializationContext> _childContexts = new Dictionary<string, SerializationContext>();

        public SerializationContext(ContextMode contextMode, IStreamSource streamSource, SerializerMode serializerMode)
        {
            ContextMode = contextMode;
            StreamSource = Argument.NotNull(streamSource, nameof(streamSource));
            SerializerMode = Argument.NotNull(serializerMode, nameof(serializerMode));
            _backgroundStream = new BackgroundStream();
        }

        private SerializationContext(ContextMode contextMode, IStreamSource streamSource, SerializerMode serializerMode, BackgroundStream parentStream)
        {
            ContextMode = contextMode;
            StreamSource = Argument.NotNull(streamSource, nameof(streamSource));
            SerializerMode = Argument.NotNull(serializerMode, nameof(serializerMode));
            _backgroundStream = Argument.NotNull(parentStream, nameof(parentStream));
        }

        public bool IsSnapShot { get; private set; }

        public ContextMode ContextMode { get; private set; }

        public IStreamSource StreamSource { get; private set; }

        public SerializerMode SerializerMode { get; private set; }

        public void Dispose()
        {
            DisposeImpl(true);
            GC.SuppressFinalize(this);
        }

        [SuppressMessage("Microsoft.Reliability", "CA2000:Objekte verwerfen, bevor Bereich verloren geht", Justification = "No Disposing Needed")]
        public SerializationContext? GetSubContext([CanBeNull] string relativeFile, ContextMode contextMode)
        {
            if (relativeFile == null) return this;

            if (_childContexts.TryGetValue(relativeFile, out var context)) return context;

            var streamSource = StreamSource.OpenSideLocation(relativeFile);
            if (streamSource == null) return null;
            if (_backgroundStream == null) return null;

            context = new SerializationContext(contextMode, streamSource, SerializerMode, _backgroundStream);
            _childContexts[relativeFile] = context;

            return context;
        }

        [SuppressMessage("Microsoft.Reliability", "CA2000:Objekte verwerfen, bevor Bereich verloren geht", Justification = "No Disposing Needed")]
        public SerializationContext CreateSnapshot(string value)
        {
            return new SerializationContext(ContextMode, new GenericSource(Encoding.UTF8.GetBytes(value), this), SerializerMode) {IsSnapShot = true};
        }

        [SuppressMessage("Microsoft.Reliability", "CA2000:Objekte verwerfen, bevor Bereich verloren geht", Justification = "No disposing Needed")]
        public SerializationContext CreateSnapshot(byte[] value)
        {
            return new SerializationContext(ContextMode, new GenericSource(Argument.NotNull(value, nameof(value)), this), SerializerMode) {IsSnapShot = true};
        }

        private Stream EnsurceBackgroundStream()
        {
            if (IsSnapShot) return StreamSource.OpenStream(FileAccess.ReadWrite);

            return _backgroundStream.Stream ??
                   (_backgroundStream.Stream = StreamSource.OpenStream(SerializerMode == SerializerMode.Deserialize
                       ? FileAccess.Read
                       : FileAccess.Write));
        }

        public static string ConvertByteToString(byte[] value)
        {
            return Base91.Encode(Argument.NotNull(value, nameof(value)));
        }

        public static byte[] ConvertStringToBytes(string value)
        {
            return Base91.Decode(Argument.NotNull(value, nameof(value)));
        }

        ~SerializationContext()
        {
            DisposeImpl(false);
        }

        private void DisposeImpl(bool disposing)
        {
            if (disposing && _childContexts != null)
            {
                StreamSource.Dispose();

                foreach (var serializationContext in _childContexts)
                    serializationContext.Value.Dispose();
            }

            _backgroundStream?.Stream?.Dispose();

            if (_binaryWriter != null)
            {
                _binaryWriter.Dispose();
                _binaryWriter = null;
            }

            if (_binaryReader != null)
            {
                _binaryReader.Dispose();
                _binaryReader = null;
            }

            if (_textReader != null)
            {
                _textReader.Dispose();
                _textReader = null;
            }

            if (_textWriter == null) return;
            _textWriter.Dispose();
            _textWriter = null;
        }

        private class GenericSource : IStreamSource
        {
            private readonly byte[] _bytes;
            private readonly SerializationContext _original;

            public GenericSource(byte[] bytes, SerializationContext original)
            {
                _bytes = Argument.NotNull(bytes, nameof(bytes));
                _original = Argument.NotNull(original, nameof(original));
            }

            public void Dispose()
            {
            }

            public Stream OpenStream(FileAccess access) 
                => new MemoryStream(_bytes);

            public IStreamSource OpenSideLocation(string? relativePath) 
                => _original.StreamSource.OpenSideLocation(relativePath) ?? EmptySource.Instance;
        }

        private class BackgroundStream
        {
            public Stream? Stream { get; set; }
        }

        #region Binary

        private BinaryReader? _binaryReader;

        public BinaryReader BinaryReader
        {
            get
            {
                if (ContextMode != ContextMode.Binary && SerializerMode != SerializerMode.Deserialize && !IsSnapShot) throw new InvalidOperationException("No Binary Deserialisation");

                return _binaryReader ??= new BinaryReader(EnsurceBackgroundStream());
            }
        }

        private BinaryWriter? _binaryWriter;

        public BinaryWriter BinaryWriter
        {
            get
            {
                if (ContextMode != ContextMode.Binary && SerializerMode != SerializerMode.Serialize && !IsSnapShot) throw new InvalidOperationException("No Binary Serialisation");

                return _binaryWriter ??= new BinaryWriter(EnsurceBackgroundStream());
            }
        }

        public void WriteBytes([CanBeNull] byte[] value)
        {
            if (value == null) return;

            BinaryWriter.Write(value.Length);
            BinaryWriter.Write(value);
        }

        public byte[] Readbytes()
        {
            return BinaryReader.ReadBytes(BinaryReader.ReadInt32());
        }

        #endregion

        #region Text

        private TextReader? _textReader;

        public TextReader TextReader
        {
            get
            {
                if (ContextMode != ContextMode.Binary && SerializerMode != SerializerMode.Deserialize) throw new InvalidOperationException("No Binary Deserialisation");

                return _textReader ??= new StreamReader(EnsurceBackgroundStream());
            }
        }

        private TextWriter? _textWriter;

        public TextWriter TextWriter
        {
            get
            {
                if (ContextMode != ContextMode.Binary && SerializerMode != SerializerMode.Serialize) throw new InvalidOperationException("No Binary Serialisation");

                return _textWriter ??= new StreamWriter(EnsurceBackgroundStream());
            }
        }

        #endregion
    }
}