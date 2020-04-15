using System;
using JetBrains.Annotations;
using Tauron.Application.Files.Serialization.Core;

namespace Tauron.Application.Files.Serialization
{
    [PublicAPI]
    public interface ISerializer
    {
        AggregateException? Errors { get; }

        void Serialize(IStreamSource target, object graph);

        object Deserialize(IStreamSource target);

        void Deserialize(IStreamSource targetStream, object target);
    }
}