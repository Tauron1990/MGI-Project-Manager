using JetBrains.Annotations;
using Tauron.Application.Files.Serialization.Core;
using Tauron.Application.Files.Serialization.Sources;

namespace Tauron.Application.Files.Serialization
{
    [PublicAPI]
    public sealed class TypedSerializer<TType>
        where TType : class
    {
        public TypedSerializer(ISerializer serializer)
        {
            Serializer = Argument.NotNull(serializer, nameof(serializer));
        }

        public ISerializer Serializer { get; }

        public void Serialize(IStreamSource source, TType graph)
        {
            Serializer.Serialize(Argument.NotNull(source, nameof(source)), Argument.NotNull(graph, nameof(graph)));
        }

        public void Deserialize(IStreamSource source, TType target)
        {
            Serializer.Deserialize(Argument.NotNull(source, nameof(source)), Argument.NotNull(target, nameof(target)));
        }

        public TType Deserialize(IStreamSource source)
        {
            return (TType) Serializer.Deserialize(Argument.NotNull(source, nameof(source)));
        }

        public void Serialize(string file, TType graph)
        {
            Serializer.Serialize(new FileSource(Argument.NotNull(file, nameof(file))), Argument.NotNull(graph, nameof(graph)));
        }

        public void Deserialize(string file, TType target)
        {
            Serializer.Deserialize(new FileSource(Argument.NotNull(file, nameof(file))), Argument.NotNull(target, nameof(target)));
        }

        public TType Deserialize([NotNull] string file)
        {
            return (TType) Serializer.Deserialize(new FileSource(file));
        }
    }
}