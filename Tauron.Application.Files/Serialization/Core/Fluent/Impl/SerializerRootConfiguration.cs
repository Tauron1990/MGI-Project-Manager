using JetBrains.Annotations;

namespace Tauron.Application.Files.Serialization.Core.Fluent.Impl
{
    internal abstract class SerializerRootConfigurationBase : ISerializerRootConfiguration
    {
        public TypedSerializer<TType> Apply<TType>() where TType : class
        {
            return new TypedSerializer<TType>(ApplyInternal());
        }

        [NotNull]
        public abstract ISerializer ApplyInternal();

        protected void VerifyErrors([NotNull] ISerializer serializer)
        {
            var aggregateException = serializer.Errors;
            if (aggregateException != null) throw aggregateException;
        }
    }
}