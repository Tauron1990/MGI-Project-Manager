// The file ExportMetadataBaseAttribute.cs is part of Tauron.Application.Common.
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
// <copyright file="ExportMetadataBaseAttribute.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The export metadata base attribute.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using JetBrains.Annotations;

#endregion

namespace Tauron.Application.Ioc
{
    /// <summary>The export metadata base attribute.</summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    [PublicAPI]
    public abstract class ExportMetadataBaseAttribute : Attribute
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ExportMetadataBaseAttribute" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="ExportMetadataBaseAttribute" /> Klasse.
        ///     Initializes a new instance of the <see cref="ExportMetadataBaseAttribute" /> class.
        /// </summary>
        /// <param name="key">
        ///     The key.
        /// </param>
        /// <param name="value">
        ///     The value.
        /// </param>
        protected ExportMetadataBaseAttribute([NotNull] string key, object value)
        {
            if (string.IsNullOrWhiteSpace(key)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(key));
            InternalKey   = key;
            InternalValue = value;
        }

        #endregion

        #region Fields

        #endregion

        #region Properties

        /// <summary>Gets or sets the internal key.</summary>
        /// <value>The internal key.</value>
        protected internal string InternalKey { get; }

        /// <summary>Gets or sets the internal value.</summary>
        /// <value>The internal value.</value>
        protected internal object InternalValue { get; set; }

        #endregion
    }
}