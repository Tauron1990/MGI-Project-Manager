// The file IMetadataFactory.cs is part of Tauron.Application.Common.
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
// <copyright file="IMetadataFactory.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The MetadataFactory interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using JetBrains.Annotations;

#endregion

namespace Tauron.Application.Ioc.BuildUp.Exports
{
    /// <summary>The MetadataFactory interface.</summary>
    [PublicAPI]
    public interface IMetadataFactory
    {
        #region Public Methods and Operators

        /// <summary>
        ///     The create metadata.
        /// </summary>
        /// <param name="interfaceType">
        ///     The interface.
        /// </param>
        /// <param name="metadata">
        ///     The metadata.
        /// </param>
        /// <returns>
        ///     The <see cref="object" />.
        /// </returns>
        object CreateMetadata(Type interfaceType, IDictionary<string, object> metadata);

        #endregion
    }
}