// The file ImportSelectorChain.cs is part of Tauron.Application.Common.
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
// <copyright file="ImportSelectorChain.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The import selector chain.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;

#endregion

namespace Tauron.Application.Ioc.BuildUp.Exports.DefaultExports
{
    /// <summary>The import selector chain.</summary>
    public class ImportSelectorChain : IImportSelectorChain
    {
        #region Fields

        /// <summary>The _selectors.</summary>
        private readonly List<IImportSelector> _selectors = new List<IImportSelector>();

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The register.
        /// </summary>
        /// <param name="selector">
        ///     The selector.
        /// </param>
        public void Register(IImportSelector selector)
        {
            _selectors.Add(selector);
        }

        /// <summary>
        ///     The select.
        /// </summary>
        /// <param name="exportType">
        ///     The export.
        /// </param>
        /// <returns>
        ///     The <see cref="IEnumerable{T}" />.
        /// </returns>
        public IEnumerable<ImportMetadata> SelectImport(IExport exportType)
        {
            var metadatas = new HashSet<ImportMetadata>();
            foreach (var importMetadata in
                _selectors.SelectMany(selector => selector.SelectImport(exportType))) metadatas.Add(importMetadata);

            return metadatas;
        }

        #endregion
    }
}