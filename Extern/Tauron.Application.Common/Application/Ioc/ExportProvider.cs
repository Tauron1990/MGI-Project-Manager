// The file ExportProvider.cs is part of Tauron.Application.Common.
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
// <copyright file="ExportProvider.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The export provider.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Tauron.Application.Ioc.BuildUp.Exports;

#endregion

namespace Tauron.Application.Ioc
{
    /// <summary>The export provider.</summary>
    [PublicAPI, Serializable]
    public abstract class ExportProvider
    {
        #region Public Events

        /// <summary>The exports changed.</summary>
        public event EventHandler<ExportChangedEventArgs> ExportsChanged;

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The create exports.
        /// </summary>
        /// <param name="factory">
        ///     The factory.
        /// </param>
        /// <returns>
        ///     The <see cref="IEnumerable" />.
        /// </returns>
        public abstract IEnumerable<Tuple<IExport, int>> CreateExports(IExportFactory factory);

        #endregion

        #region Methods

        /// <summary>
        ///     The on exports changed.
        /// </summary>
        /// <param name="e">
        ///     The e.
        /// </param>
        protected virtual void OnExportsChanged([NotNull] ExportChangedEventArgs e)
        {
            if (e == null) throw new ArgumentNullException(nameof(e));
            var handler = ExportsChanged;
            handler?.Invoke(this, e);
        }

        #endregion

        #region Public Properties

        /// <summary>Gets a value indicating whether broadcast changes.</summary>
        /// <value>The broadcast changes.</value>
        public virtual bool BroadcastChanges => false;

        /// <summary>Gets the technology.</summary>
        /// <value>The technology.</value>
        public abstract string Technology { get; }

        #endregion
    }

    /// <summary>The export changed event args.</summary>
    [PublicAPI, Serializable]
    public class ExportChangedEventArgs : EventArgs
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ExportChangedEventArgs" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="ExportChangedEventArgs" /> Klasse.
        /// </summary>
        /// <param name="addedExport">
        ///     The added export.
        /// </param>
        /// <param name="removedExport">
        ///     The removed export.
        /// </param>
        public ExportChangedEventArgs(
            [NotNull] IEnumerable<ExportMetadata> addedExport,
            [NotNull] IEnumerable<ExportMetadata> removedExport)
        {
            Added   = addedExport ?? throw new ArgumentNullException(nameof(addedExport));
            Removed = removedExport ?? throw new ArgumentNullException(nameof(removedExport));
        }

        #endregion

        #region Fields

        #endregion

        #region Public Properties

        /// <summary>Gets or sets the added.</summary>
        /// <value>The added.</value>
        public IEnumerable<ExportMetadata> Added { get; }

        /// <summary>Gets or sets the removed.</summary>
        /// <value>The removed.</value>
        public IEnumerable<ExportMetadata> Removed { get; }

        #endregion
    }
}