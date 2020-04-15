﻿using System;
using System.IO;
using JetBrains.Annotations;
using Tauron.Application.Files.Serialization.Core.Managment;

namespace Tauron.Application.Files.Serialization.Core.Impl.Mapper
{
    internal class SerializerBindMapper<TContext> : MappingEntryBase<TContext>
        where TContext : IOrginalContextProvider
    {
        internal class StreamManager : IStreamSource
        {
            private readonly Func<object, SerializerMode, Stream> _open;
            private readonly Func<string, IStreamSource> _openRelative;

            private Stream _current;
            private SerializerMode _mode;

            private object _target;

            public StreamManager([CanBeNull] Func<object, SerializerMode, Stream> open, [CanBeNull] Func<string, IStreamSource> openRelative)
            {
                _open = open;
                _openRelative = openRelative;
            }

            public void Dispose()
            {
                if (_current == null)
                    return;

                _current.Dispose();
                _current = null;
                _target = null;
            }

            public Stream OpenStream(FileAccess access) => OpenStream(_target, _mode);

            public IStreamSource OpenSideLocation(string relativePath)
            {
                if (_openRelative != null)
                    return _openRelative(relativePath);

                throw new NotSupportedException();
            }

            [NotNull]
            private Stream OpenStream([NotNull] object target, SerializerMode mode)
            {
                if (_current != null)
                    throw new InvalidOperationException();

                _current = _open(target, mode);
                return _current;
            }

            [CanBeNull]
            public Exception VerifyError() => _open == null ? new SerializerElementException("Open Function") : null;

            public void Initialize([NotNull] object target, SerializerMode mode)
            {
                _target = Argument.NotNull(target, nameof(target));
                _mode = mode;
            }
        }

        private readonly StreamManager _manager;
        private readonly ISerializer _serializer;

        public SerializerBindMapper([CanBeNull] string membername, [NotNull] Type targetType, [CanBeNull] ISerializer serializer, [CanBeNull] StreamManager manager) 
            : base(membername, targetType)
        {
            _serializer = serializer;
            _manager = manager;
        }

        protected override void Deserialize(object target, TContext context)
        {
            using (_manager)
            {
                _manager.Initialize(target, SerializerMode.Deserialize);
                SetValue(target, _serializer.Deserialize(_manager));
            }
        }

        protected override void Serialize(object target, TContext context)
        {
            using (_manager)
            {
                _manager.Initialize(target, SerializerMode.Serialize);
                _serializer.Serialize(_manager, GetValue(target));
            }
        }

        public override Exception VerifyError()
        {
            var e = base.VerifyError();

            if (e != null) return e;

            e = _manager == null ? new ArgumentNullException(nameof(_manager), @"Open Stream") : _manager.VerifyError();

            if (_serializer == null)
                e = new ArgumentNullException(nameof(_serializer), @"Serializer");

            return e;
        }
    }
}