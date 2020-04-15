using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Tauron.Application.Files.Serialization.Core.Managment
{
    [PublicAPI]
    public abstract class SerializerBase<TContext> : ISubSerializer
        where TContext : class, IOrginalContextProvider
    {
        private readonly ObjectBuilder          _builder;
        private readonly ContextMode            _contextMode;
        private readonly SimpleMapper<TContext> _mapper;

        protected SerializerBase([NotNull] ObjectBuilder builder, [NotNull] SimpleMapper<TContext> mapper, ContextMode contextMode)
        {
            _builder     = Argument.NotNull(builder, nameof(builder));
            _mapper      = Argument.NotNull(mapper, nameof(mapper));
            _contextMode = contextMode;
        }

        public virtual AggregateException Errors
        {
            get
            {
                var errors = new List<Exception>();
                var e      = _builder.Verfiy();

                if (e != null) errors.Add(e);

                foreach (var mappingEntry in _mapper.Entries)
                {
                    e = mappingEntry.VerifyError();
                    if (e != null) errors.Add(e);
                }

                return errors.Count != 0 ? new AggregateException(errors) : null;
            }
        }

        public virtual void Serialize(IStreamSource target, object graph) => Progress(graph, target, SerializerMode.Serialize);

        public virtual object Deserialize(IStreamSource target)
        {
            var garph = BuildObject();
            Progress(garph, target, SerializerMode.Deserialize);

            return garph;
        }

        public virtual void Deserialize(IStreamSource targetStream, object target) => Progress(target, targetStream, SerializerMode.Deserialize);

        void ISubSerializer.Serialize(SerializationContext target, object graph)
        {
            var context = BuildContext(target);

            foreach (var mappingEntry in _mapper.Entries)
                mappingEntry.Progress(graph, context, SerializerMode.Serialize);
        }

        object ISubSerializer.Deserialize(SerializationContext target)
        {
            var obj     = BuildObject();
            var context = BuildContext(target);

            foreach (var mappingEntry in _mapper.Entries)
                mappingEntry.Progress(obj, context, SerializerMode.Deserialize);

            return obj;
        }

        [NotNull]
        protected object BuildObject() => Argument.CheckResult(_builder?.BuilderFunc?.Invoke(_builder.CustomObject), "Object Build Was null");

        private void Progress([NotNull] object graph, [NotNull] IStreamSource target, SerializerMode mode)
        {
            var context = BuildContext(new SerializationContext(_contextMode, target, mode));
            Progress(graph, context, mode);
        }

        public void Progress([NotNull] object graph, [NotNull] TContext context, SerializerMode mode)
        {
            foreach (var mappingEntry in _mapper.Entries)
                mappingEntry.Progress(graph, context, mode);

            CleanUp(context);
        }

        [NotNull]
        public abstract TContext BuildContext([NotNull] SerializationContext context);

        public abstract void CleanUp([NotNull] TContext context);
    }
}