using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Resources;
using JetBrains.Annotations;

namespace Tauron.Application
{
    [PublicAPI]
    public static class ResourceManagerProvider
    {
        private static readonly Dictionary<Assembly, ResourceManager> Resources =
            new Dictionary<Assembly, ResourceManager>();
        
        public static void Register([NotNull] ResourceManager manager, [NotNull] Assembly key) => Resources[Argument.NotNull(key, nameof(key))] = Argument.NotNull(manager, nameof(manager));

        public static void Remove([NotNull] Assembly key) => Resources.Remove(key);

        public static string? FindResource([NotNull]string name, [CanBeNull]Assembly key, bool searchEverywere = true)
        {
            Argument.NotNull(name, nameof(name));

            if (key != null && Resources.TryGetValue(key, out var rm))
                return rm.GetString(name);

            return searchEverywere ? Resources.Select(rm2 => rm2.Value.GetString(name)).FirstOrDefault(str => !string.IsNullOrWhiteSpace(str)) : null;
        }
    }
}