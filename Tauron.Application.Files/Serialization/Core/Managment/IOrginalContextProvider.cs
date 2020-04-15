using JetBrains.Annotations;

namespace Tauron.Application.Files.Serialization.Core.Managment
{
    public interface IOrginalContextProvider
    {
        [NotNull] SerializationContext Original { get; }
    }
}