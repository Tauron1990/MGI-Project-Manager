using System;
using System.IO;
using JetBrains.Annotations;

namespace Tauron.Application.Files.Serialization.Core.Fluent
{
    [PublicAPI]
    public interface ISerializerToMemberConfiguration<out TConfigInterface>
        where TConfigInterface : class
    {
        ISerializerToMemberConfiguration<TConfigInterface> WithSourceSelector(Func<object, SerializerMode, Stream> open, Func<string?, IStreamSource>? openRelative);

        ISerializerToMemberConfiguration<TConfigInterface> WithSerializer(ISerializer serializer);

        TConfigInterface Apply();
    }
}