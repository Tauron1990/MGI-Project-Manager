using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using JetBrains.Annotations;

namespace Tauron.Application.Files.Serialization.Core.Fluent
{
    public interface IBinaryConfiguration : ISerializerRootConfiguration
    {
        [NotNull]
        IBinaryConfiguration WithFormat(FormatterAssemblyStyle format);

        [NotNull]
        IBinaryConfiguration WithBinder([NotNull] SerializationBinder binder);

        [NotNull]
        IBinaryConfiguration WithContext(StreamingContext context);

        [NotNull]
        IBinaryConfiguration WithFilterLevel(TypeFilterLevel level);

        [NotNull]
        IBinaryConfiguration WithSelector([NotNull] ISurrogateSelector selector);

        [NotNull]
        IBinaryConfiguration WithFormat(FormatterTypeStyle format);
    }
}