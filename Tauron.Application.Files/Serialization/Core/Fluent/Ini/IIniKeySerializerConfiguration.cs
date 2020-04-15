using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using Tauron.Application.Files.Serialization.Core.Managment;

namespace Tauron.Application.Files.Serialization.Core.Fluent
{
    public interface IIniKeySerializerConfiguration : IWithMember<IIniKeySerializerConfiguration>
    {
        [NotNull]
        IIniKeySerializerConfiguration WithKey([CanBeNull] string name);

        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "No simpler way")]
        [NotNull]
        IIniKeySerializerConfiguration WithConverter([CanBeNull] SimpleConverter<IEnumerable<string>> converter);

        [NotNull]
        IIniSerializerConfiguration Apply();
    }
}