// The file IImportSelectorChain.cs is part of Tauron.Application.Common.
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
// <copyright file="IImportSelectorChain.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The ImportSelectorChain interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using JetBrains.Annotations;

#endregion

namespace Tauron.Application.Ioc.BuildUp.Exports.DefaultExports
{
    /// <summary>The ImportSelectorChain interface.</summary>
    [PublicAPI]
    public interface IImportSelectorChain : IImportSelector
    {
        #region Public Methods and Operators

        /// <summary>
        ///     The register.
        /// </summary>
        /// <param name="selector">
        ///     The selector.
        /// </param>
        void Register(IImportSelector selector);

        #endregion
    }
}