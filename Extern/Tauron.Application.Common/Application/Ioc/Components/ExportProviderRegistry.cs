// The file ExportProviderRegistry.cs is part of Tauron.Application.Common.
// 
// CoreEngine is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// CoreEngine is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//  
// You should have received a copy of the GNU General Public License
//  along with Tauron.Application.Common If not, see <http://www.gnu.org/licenses/>.

#region

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExportProviderRegistry.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The export provider registry.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using JetBrains.Annotations;

#endregion

namespace Tauron.Application.Ioc.Components
{
    /// <summary>The export provider registry.</summary>
    [PublicAPI]
    public sealed class ExportProviderRegistry : IDisposable
    {
        #region Fields

        /// <summary>The _providers.</summary>
        private readonly List<ExportProvider> _providers;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ExportProviderRegistry" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="ExportProviderRegistry" /> Klasse.
        ///     Initializes a new instance of the <see cref="ExportProviderRegistry" /> class.
        /// </summary>
        public ExportProviderRegistry()
        {
            _providers = new List<ExportProvider>();
        }

        #endregion

        #region Public Events

        /// <summary>The exports changed.</summary>
        public event EventHandler<ExportChangedEventArgs> ExportsChanged;

        #endregion

        #region Methods

        /// <summary>
        ///     The on exports changed.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="e">
        ///     The e.
        /// </param>
        private void OnExportsChanged([NotNull] object sender, ExportChangedEventArgs e)
        {
            if (sender == null) throw new ArgumentNullException(nameof(sender));
            var handler = ExportsChanged;
            handler?.Invoke(sender, e);
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>The dispose.</summary>
        public void Dispose()
        {
            foreach (var exportProvider in _providers)
            {
                if (exportProvider is IDisposable dipo) dipo.Dispose();
            }
        }

        /// <summary>
        ///     The add.
        /// </summary>
        /// <param name="provider">
        ///     The provider.
        /// </param>
        public void Add([NotNull] ExportProvider provider)
        {
            if (provider == null) throw new ArgumentNullException(nameof(provider));
            lock (_providers)
            {
                _providers.Add(provider);
                provider.ExportsChanged += OnExportsChanged;
            }
        }

        #endregion
    }
}