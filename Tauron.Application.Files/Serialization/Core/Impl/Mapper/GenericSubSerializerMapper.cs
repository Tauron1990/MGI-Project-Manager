using System;
using JetBrains.Annotations;
using Tauron.Application.Files.Serialization.Core.Managment;

namespace Tauron.Application.Files.Serialization.Core.Impl.Mapper
{
    [PublicAPI]
    public abstract class GenericSubSerializerMapper<TContext> : MappingEntryBase<TContext>
        where TContext : class, IOrginalContextProvider
    {
        private readonly ISubSerializer _serializer;

        protected GenericSubSerializerMapper([CanBeNull] string membername, [NotNull] Type targetType, [CanBeNull] ISubSerializer serializer)
            : base(membername, targetType) => _serializer = serializer;

        protected override void Deserialize(object target, TContext context) 
            => SetValue(Argument.NotNull(target, nameof(target)), _serializer.Deserialize(GetRealContext(Argument.NotNull(context, nameof(context)), SerializerMode.Deserialize)));

        protected override void Serialize(object target, TContext context) 
            => _serializer.Serialize(GetRealContext(Argument.NotNull(context, nameof(context)), SerializerMode.Serialize), GetValue(Argument.NotNull(target, nameof(target))));

        public override Exception VerifyError()
        {
            var e = base.VerifyError();

            if (_serializer == null)
                e = new SerializerElementNullException("The Serializer does not Support the SupSerializer Interface");

            return e;
        }

        [NotNull]
        protected virtual SerializationContext GetRealContext([NotNull] TContext origial, SerializerMode mode) => origial.Original;

        protected virtual void PostProgressing([NotNull] SerializationContext context) { }
    }
}