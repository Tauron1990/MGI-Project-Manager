using JetBrains.Annotations;
using Tauron.Application.Files.Serialization.Core;
using Tauron.Application.Files.Serialization.Sources;

namespace Tauron.Application.Files.Serialization
{
    [PublicAPI]
    public sealed class TypedSerializer<TType>
        where TType : class
    {
        public TypedSerializer([NotNull] ISerializer serializer) => Serializer = Argument.NotNull(serializer, nameof(serializer));

        [NotNull]
        public ISerializer Serializer { get; }
        
        public void Serialize([NotNull] IStreamSource source, [NotNull] TType graph) 
            => Serializer.Serialize(Argument.NotNull(source, nameof(source)), Argument.NotNull(graph, nameof(graph)));

        public void Deserialize([NotNull] IStreamSource source, [NotNull] TType target) 
            => Serializer.Deserialize(Argument.NotNull(source, nameof(source)), Argument.NotNull(target, nameof(target)));

        [NotNull]
        public TType Deserialize([NotNull] IStreamSource source) => (TType) Serializer.Deserialize(Argument.NotNull(source, nameof(source)));

        public void Serialize([NotNull] string file, [NotNull] TType graph)
            => Serializer.Serialize(new FileSource(Argument.NotNull(file, nameof(file))), Argument.NotNull(graph, nameof(graph)));

        public void Deserialize([NotNull] string file, [NotNull] TType target) 
            => Serializer.Deserialize(new FileSource(Argument.NotNull(file, nameof(file))), Argument.NotNull(target, nameof(target)));

        [NotNull]
        public TType Deserialize([NotNull] string file) => (TType) Serializer.Deserialize(new FileSource(file));
    }
}