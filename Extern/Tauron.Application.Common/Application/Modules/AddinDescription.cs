using System;
using JetBrains.Annotations;

namespace Tauron.Application.Modules
{
    public sealed class AddinDescription
    {
        public AddinDescription([NotNull] Version version, [NotNull] string description, [NotNull] string name)
        {
            if (version == null) throw new ArgumentNullException(nameof(version));
            Version     = version;
            Description = description ?? throw new ArgumentNullException(nameof(description));
            Name        = name ?? throw new ArgumentNullException(nameof(name));
        }

        [NotNull]
        public Version Version { get; set; }

        [NotNull]
        public string Description { get; set; }

        [NotNull]
        public string Name { get; set; }
    }
}