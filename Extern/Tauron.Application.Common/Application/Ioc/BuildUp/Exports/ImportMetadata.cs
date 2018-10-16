// The file ImportMetadata.cs is part of Tauron.Application.Common.
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
// <copyright file="ImportMetadata.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The import metadata.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using JetBrains.Annotations;

#endregion

namespace Tauron.Application.Ioc.BuildUp.Exports
{
    /// <summary>The import metadata.</summary>
    [PublicAPI]
    public sealed class ImportMetadata : IEquatable<ImportMetadata>
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ImportMetadata" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="ImportMetadata" /> Klasse.
        ///     Initializes a new instance of the <see cref="ImportMetadata" /> class.
        /// </summary>
        /// <param name="interfaceType">
        ///     The interface type.
        /// </param>
        /// <param name="contractName">
        ///     The contract name.
        /// </param>
        /// <param name="export">
        ///     The export.
        /// </param>
        /// <param name="memberName">
        ///     The member name.
        /// </param>
        /// <param name="optional">
        ///     The optional.
        /// </param>
        /// <param name="metadata">
        ///     The metadata.
        /// </param>
        public ImportMetadata(
            [CanBeNull] Type interfaceType,
            [CanBeNull] string contractName,
            [NotNull] IExport export,
            [NotNull] string memberName,
            bool optional,
            [NotNull] IDictionary<string, object> metadata)
        {
            if (export == null) throw new ArgumentNullException(nameof(export));
            if (metadata == null) throw new ArgumentNullException(nameof(metadata));
            if (string.IsNullOrWhiteSpace(memberName))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(memberName));
            InterfaceType = interfaceType;
            ContractName = contractName;
            Export = export;
            MemberName = memberName;
            Optional = optional;
            Metadata = metadata;
        }

        #endregion

        #region Fields

        #endregion

        #region Public Properties

        /// <summary>Gets the contract name.</summary>
        /// <value>The contract name.</value>
        public string ContractName { get; private set; }

        /// <summary>Gets the export.</summary>
        /// <value>The export.</value>
        public IExport Export { get; private set; }

        /// <summary>Gets the interface type.</summary>
        /// <value>The interface type.</value>
        public Type InterfaceType { get; private set; }

        /// <summary>Gets the member name.</summary>
        /// <value>The member name.</value>
        public string MemberName { get; private set; }

        /// <summary>Gets the metadata.</summary>
        /// <value>The metadata.</value>
        public IDictionary<string, object> Metadata { get; private set; }

        /// <summary>Gets a value indicating whether optional.</summary>
        /// <value>The optional.</value>
        public bool Optional { get; private set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The equals.
        /// </summary>
        /// <param name="other">
        ///     The other.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public bool Equals(ImportMetadata other)
        {
            if (ReferenceEquals(null, other)) return false;

            if (ReferenceEquals(this, other)) return true;

            bool flag;
            if (InterfaceType != null) flag = InterfaceType == other.InterfaceType;
            else flag = other.InterfaceType == null;

            return flag && string.Equals(ContractName, other.ContractName)
                        && string.Equals(MemberName, other.MemberName);
        }

        /// <summary>The ==.</summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns></returns>
        public static bool operator ==(ImportMetadata left, ImportMetadata right)
        {
            return Equals(left, right);
        }

        /// <summary>The !=.</summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns></returns>
        public static bool operator !=(ImportMetadata left, ImportMetadata right)
        {
            return !Equals(left, right);
        }

        /// <summary>
        ///     The equals.
        /// </summary>
        /// <param name="obj">
        ///     The obj.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public override bool Equals(object obj)
        {
            var met = obj as ImportMetadata;
            return met != null && Equals(met);
        }

        /// <summary>The get hash code.</summary>
        /// <returns>
        ///     The <see cref="int" />.
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return (InterfaceType?.GetHashCode() * 397 ?? 0)
                       ^ (ContractName?.GetHashCode() ?? 0);
            }
        }

        [DebuggerStepThrough]
        public override string ToString()
        {
            return ErrorTracer.FormatExport(InterfaceType, ContractName);
        }

        #endregion
    }
}