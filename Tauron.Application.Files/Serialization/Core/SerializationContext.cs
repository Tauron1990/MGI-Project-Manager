using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using JetBrains.Annotations;

namespace Tauron.Application.Files.Serialization.Core
{
    [PublicAPI]
    public sealed class SerializationContext : IDisposable
    {
        private class GenericSource : IStreamSource
        {
            private byte[] _bytes;
            private SerializationContext _original;

            public GenericSource([NotNull] byte[] bytes, [NotNull] SerializationContext original)
            {
                _bytes = Argument.NotNull(bytes, nameof(bytes));
                _original = Argument.NotNull(original, nameof(original));
            }

            public void Dispose()
            {
                _bytes = null;
                _original = null;
            }

            public Stream OpenStream(FileAccess access) => new MemoryStream(_bytes);

            public IStreamSource OpenSideLocation(string relativePath) => _original.StreamSource.OpenSideLocation(relativePath);
        }

        private class BackgroundStream
        {
            [CanBeNull]
            public Stream Stream { get; set; }
        }

        private BackgroundStream _backgroundStream;

        private Dictionary<string, SerializationContext> _childContexts;

        public SerializationContext(ContextMode contextMode, [NotNull] IStreamSource streamSource, SerializerMode serializerMode)
        {
            ContextMode = contextMode;
            StreamSource = Argument.NotNull(streamSource, nameof(streamSource));
            SerializerMode = Argument.NotNull(serializerMode, nameof(serializerMode));
            _backgroundStream = new BackgroundStream();
        }

        private SerializationContext(ContextMode contextMode, [NotNull] IStreamSource streamSource, SerializerMode serializerMode, [NotNull] BackgroundStream parentStream)
        {
            ContextMode = contextMode;
            StreamSource = Argument.NotNull(streamSource, nameof(streamSource));
            SerializerMode = Argument.NotNull(serializerMode, nameof(serializerMode));
            _backgroundStream = Argument.NotNull(parentStream, nameof(parentStream));
        }

        public bool IsSnapShot { get; private set; }

        public ContextMode ContextMode { get; private set; }

        [NotNull]
        public IStreamSource StreamSource { get; private set; }

        public SerializerMode SerializerMode { get; private set; }

        public void Dispose()
        {
            DisposeImpl(true);
            GC.SuppressFinalize(this);
        }

        [SuppressMessage("Microsoft.Reliability", "CA2000:Objekte verwerfen, bevor Bereich verloren geht", Justification = "No Disposing Needed")]
        [CanBeNull]
        public SerializationContext GetSubContext([CanBeNull] string relativeFile, ContextMode contextMode)
        {
            if (relativeFile == null) return this;

            if (_childContexts.TryGetValue(relativeFile, out var context)) return context;

            var streamSource = StreamSource.OpenSideLocation(relativeFile);
            if (streamSource == null) return null;

            context = new SerializationContext(contextMode, streamSource, SerializerMode, _backgroundStream);
            _childContexts[relativeFile] = context;

            return context;
        }

        [SuppressMessage("Microsoft.Reliability", "CA2000:Objekte verwerfen, bevor Bereich verloren geht", Justification = "No Disposing Needed")]
        [NotNull]
        public SerializationContext CreateSnapshot([NotNull] string value) 
            => new SerializationContext(ContextMode, new GenericSource(Encoding.UTF8.GetBytes(value), this), SerializerMode) {IsSnapShot = true};

        [SuppressMessage("Microsoft.Reliability", "CA2000:Objekte verwerfen, bevor Bereich verloren geht", Justification = "No disposing Needed")]
        [NotNull]
        public SerializationContext CreateSnapshot([NotNull] byte[] value) 
            => new SerializationContext(ContextMode, new GenericSource(Argument.NotNull(value, nameof(value)), this), SerializerMode) {IsSnapShot = true};

        [NotNull]
        private Stream EnsurceBackgroundStream()
        {
            if (IsSnapShot) return StreamSource.OpenStream(FileAccess.ReadWrite);

            return _backgroundStream.Stream ??
                   (_backgroundStream.Stream = StreamSource.OpenStream(SerializerMode == SerializerMode.Deserialize
                       ? FileAccess.Read
                       : FileAccess.Write));
        }

        [NotNull]
        public static string ConvertByteToString([NotNull] byte[] value) => Base91.Encode(Argument.NotNull(value, nameof(value)));

        [NotNull]
        public static byte[] ConvertStringToBytes([NotNull] string value) => Base91.Decode(Argument.NotNull(value, nameof(value)));

        ~SerializationContext() => DisposeImpl(false);

        private void DisposeImpl(bool disposing)
        {
            if (disposing && _childContexts != null)
            {
                StreamSource.Dispose();

                foreach (var serializationContext in _childContexts)
                    serializationContext.Value.Dispose();
            }

            _backgroundStream?.Stream?.Dispose();

            _backgroundStream = null;

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

        #region Binary

        private BinaryReader _binaryReader;

        [NotNull]
        public BinaryReader BinaryReader
        {
            get
            {
                if (ContextMode != ContextMode.Binary && SerializerMode != SerializerMode.Deserialize && !IsSnapShot) throw new InvalidOperationException("No Binary Deserialisation");

                return _binaryReader ?? (_binaryReader = new BinaryReader(EnsurceBackgroundStream()));
            }
        }

        private BinaryWriter _binaryWriter;

        [NotNull]
        public BinaryWriter BinaryWriter
        {
            get
            {
                if (ContextMode != ContextMode.Binary && SerializerMode != SerializerMode.Serialize && !IsSnapShot) throw new InvalidOperationException("No Binary Serialisation");

                return _binaryWriter ?? (_binaryWriter = new BinaryWriter(EnsurceBackgroundStream()));
            }
        }

        public void WriteBytes([CanBeNull] byte[] value)
        {
            if (value == null) return;

            BinaryWriter.Write(value.Length);
            BinaryWriter.Write(value);
        }

        [NotNull]
        public byte[] Readbytes() => BinaryReader.ReadBytes(BinaryReader.ReadInt32());

        #endregion

        #region Text

        private TextReader _textReader;

        [NotNull]
        public TextReader TextReader
        {
            get
            {
                if (ContextMode != ContextMode.Binary && SerializerMode != SerializerMode.Deserialize) throw new InvalidOperationException("No Binary Deserialisation");

                return _textReader ?? (_textReader = new StreamReader(EnsurceBackgroundStream()));
            }
        }

        private TextWriter _textWriter;

        [NotNull]
        public TextWriter TextWriter
        {
            get
            {
                if (ContextMode != ContextMode.Binary && SerializerMode != SerializerMode.Serialize) throw new InvalidOperationException("No Binary Serialisation");

                return _textWriter ?? (_textWriter = new StreamWriter(EnsurceBackgroundStream()));
            }
        }

        #endregion
    }
}