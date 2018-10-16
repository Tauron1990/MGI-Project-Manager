using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Tauron.Application.Views.Core
{
    public static class NamingHelper
    {
        [NotNull]
        public static IEnumerable<string> CreatePossibilyNames([NotNull] string baseName)
        {
            if (string.IsNullOrEmpty(baseName))
                throw new ArgumentException("Value cannot be null or empty.", nameof(baseName));
            yield return baseName;
            yield return baseName + "View";
            yield return baseName + "ViewModel";

            if (!baseName.EndsWith("Model")) yield break;
            baseName = baseName.Remove(baseName.Length - 5);

            yield return baseName;
            yield return baseName + "View";
            yield return baseName + "ViewModel";
        }
    }
}