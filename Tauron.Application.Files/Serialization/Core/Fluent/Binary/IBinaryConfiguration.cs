using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using JetBrains.Annotations;

namespace Tauron.Application.Files.Serialization.Core.Fluent
{
    [PublicAPI]
    public interface IBinaryConfiguration : ISerializerRootConfiguration
    {
        IBinaryConfiguration WithFormat(FormatterAssemblyStyle format);

        IBinaryConfiguration WithBinder(SerializationBinder binder);

        IBinaryConfiguration WithContext(StreamingContext context);

        IBinaryConfiguration WithFilterLevel(TypeFilterLevel level);

        IBinaryConfiguration WithSelector(ISurrogateSelector selector);

        IBinaryConfiguration WithFormat(FormatterTypeStyle format);
    }
}