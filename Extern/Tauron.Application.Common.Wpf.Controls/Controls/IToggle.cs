// The file IToggle.cs is part of Tauron.Application.Common.Wpf.Controls.
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
//  along with Tauron.Application.Common.Wpf.Controls If not, see <http://www.gnu.org/licenses/>.

#region

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IToggle.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The Toggle interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;

#endregion

namespace Tauron.Application.Controls
{
    /// <summary>The Toggle interface.</summary>
    public interface IToggle
    {
        #region Public Events

        /// <summary>The switched.</summary>
        event Action<IToggle, bool> Switched;

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The set header.
        /// </summary>
        /// <param name="header">
        ///     The header.
        /// </param>
        void SetHeader(object header);

        /// <summary>The switch.</summary>
        void Switch();

        #endregion
    }
}