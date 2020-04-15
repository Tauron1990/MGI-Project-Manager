using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using Tauron.Application.Files.Serialization.Core.Managment;

namespace Tauron.Application.Files.Serialization.Core.Fluent
{
    [PublicAPI]
    public interface IIniKeySerializerConfiguration : IWithMember<IIniKeySerializerConfiguration>
    {
        IIniKeySerializerConfiguration WithKey(string? name);

        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "No simpler way")]
        IIniKeySerializerConfiguration WithConverter(SimpleConverter<IEnumerable<string>>? converter);

        IIniSerializerConfiguration Apply();
    }
}