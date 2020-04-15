using System;
using System.IO;
using JetBrains.Annotations;

namespace Tauron.Application.Files.Serialization.Core.Fluent
{
    [PublicAPI]
    public interface ISerializerToMemberConfiguration<out TConfigInterface>
        where TConfigInterface : class
    {
        [NotNull]
        ISerializerToMemberConfiguration<TConfigInterface> WithSourceSelector([NotNull] Func<object, SerializerMode, Stream> open, [CanBeNull] Func<string, IStreamSource> openRelative);

        [NotNull]
        ISerializerToMemberConfiguration<TConfigInterface> WithSerializer([NotNull] ISerializer serializer);

        [NotNull]
        TConfigInterface Apply();
    }
}