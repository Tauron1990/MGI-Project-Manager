#region

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Resources;
using System.Windows.Markup;
using System.Xaml;
using JetBrains.Annotations;

#endregion

namespace Tauron.Application
{
    /// <summary>The simple localize.</summary>
    [PublicAPI]
    public class SimpleLocalize : MarkupExtension
    {
        #region Static Fields

        private static readonly Dictionary<Assembly, ResourceManager> Resources =
            new Dictionary<Assembly, ResourceManager>();

        #endregion

        public SimpleLocalize([NotNull] string name)
        {
            Name = name;
        }

        public SimpleLocalize()
        {
        }

        #region Public Properties

        /// <summary>Gets or sets the name.</summary>
        [NotNull]
        public string Name { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The register.
        /// </summary>
        /// <param name="manager">
        ///     The manager.
        /// </param>
        /// <param name="key">
        ///     The key.
        /// </param>
        public static void Register([NotNull] ResourceManager manager, [NotNull] Assembly key)
        {
            if (manager == null) throw new ArgumentNullException(nameof(manager));
            if (key == null) throw new ArgumentNullException(nameof(key));
            Resources[key] = manager;
        }

        /// <summary>
        ///     The remove.
        /// </summary>
        /// <param name="key">
        ///     The key.
        /// </param>
        public static void Remove([NotNull] Assembly key)
        {
            Resources.Remove(key);
        }

        /// <summary>
        ///     The provide value.
        /// </summary>
        /// <param name="serviceProvider">
        ///     The service provider.
        /// </param>
        /// <returns>
        ///     The <see cref="object" />.
        /// </returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var provider = serviceProvider.GetService(typeof(IRootObjectProvider)) as IRootObjectProvider;

            if (provider?.RootObject == null) return Name; // "IRootObjectProvider oder das RootObject existieren nicht!";

            return Resources.TryGetValue(provider.RootObject.GetType().Assembly, out var manager)
                       ? manager.GetObject(Name)
                       : Name;
        }

        #endregion
    }
}