// The file ExportMetadataAttribute.cs is part of Tauron.Application.Common.
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
// <copyright file="ExportMetadataAttribute.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The export metadata attribute.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using JetBrains.Annotations;

#endregion

namespace Tauron.Application.Ioc
{
    /// <summary>The export metadata attribute.</summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    [PublicAPI]
    public sealed class ExportMetadataAttribute : ExportMetadataBaseAttribute
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ExportMetadataAttribute" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="ExportMetadataAttribute" /> Klasse.
        ///     Initializes a new instance of the <see cref="ExportMetadataAttribute" /> class.
        /// </summary>
        /// <param name="key">
        ///     The key.
        /// </param>
        /// <param name="value">
        ///     The value.
        /// </param>
        public ExportMetadataAttribute(string key, object value)
            : base(key, value)
        {
        }

        #endregion

        #region Public Properties

        /// <summary>Gets the key.</summary>
        /// <value>The key.</value>
        public string Key => InternalKey;

        /// <summary>Gets the value.</summary>
        /// <value>The value.</value>
        public object Value => InternalValue;

        #endregion
    }
}