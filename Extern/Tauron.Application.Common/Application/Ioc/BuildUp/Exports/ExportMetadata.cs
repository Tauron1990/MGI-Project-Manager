// The file ExportMetadata.cs is part of Tauron.Application.Common.
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
// <copyright file="ExportMetadata.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The export metadata.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using JetBrains.Annotations;

#endregion

namespace Tauron.Application.Ioc.BuildUp.Exports
{
    /// <summary>The export metadata.</summary>
    [PublicAPI]
    public sealed class ExportMetadata : IEquatable<ExportMetadata>
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initialisiert eine neue Instanz der <see cref="ExportMetadata" /> Klasse.
        /// </summary>
        /// <param name="interfaceType">
        ///     The interface type.
        /// </param>
        /// <param name="contractName">
        ///     The contract name.
        /// </param>
        /// <param name="lifetime">
        ///     The lifetime.
        /// </param>
        /// <param name="metadata">
        ///     The metadata.
        /// </param>
        /// <param name="export">
        ///     The export.
        /// </param>
        public ExportMetadata([NotNull] Type interfaceType, [CanBeNull] string contractName, [NotNull] Type lifetime,
            [NotNull] Dictionary<string, object> metadata, [NotNull] IExport export)
        {
            if (interfaceType == null) throw new ArgumentNullException(nameof(interfaceType));
            if (lifetime == null) throw new ArgumentNullException(nameof(lifetime));
            if (metadata == null) throw new ArgumentNullException(nameof(metadata));
            if (export == null) throw new ArgumentNullException(nameof(export));
            InterfaceType = interfaceType;
            ContractName = contractName;
            Lifetime = lifetime;
            Metadata = metadata;
            Export = export;
        }

        #endregion

        #region Fields

        #endregion

        #region Public Properties

        /// <summary>Gets the contract name.</summary>
        /// <value>The contract name.</value>
        [CanBeNull]
        public string ContractName { get; private set; }

        /// <summary>Gets or sets the export.</summary>
        /// <value>The export.</value>
        [NotNull]
        public IExport Export { get; set; }

        /// <summary>Gets the interface type.</summary>
        /// <value>The interface type.</value>
        [NotNull]
        public Type InterfaceType { get; private set; }

        /// <summary>Gets the lifetime.</summary>
        /// <value>The lifetime.</value>
        [NotNull]
        public Type Lifetime { get; private set; }

        /// <summary>Gets the metadata.</summary>
        /// <value>The metadata.</value>
        [NotNull]
        public Dictionary<string, object> Metadata { get; private set; }

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
        public bool Equals([CanBeNull] ExportMetadata other)
        {
            if (ReferenceEquals(null, other)) return false;

            if (ReferenceEquals(this, other)) return true;

            return InterfaceType == other.InterfaceType && string.Equals(ContractName, other.ContractName)
                   && Lifetime == other.Lifetime && Export.Equals(other);
        }

        /// <summary>The ==.</summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns></returns>
        public static bool operator ==(ExportMetadata left, ExportMetadata right)
        {
            return Equals(left, right);
        }

        /// <summary>The !=.</summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns></returns>
        public static bool operator !=(ExportMetadata left, ExportMetadata right)
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
        public override bool Equals([CanBeNull] object obj)
        {
            if (ReferenceEquals(null, obj)) return false;

            if (ReferenceEquals(this, obj)) return true;

            var meta = obj as ExportMetadata;

            return meta != null && Equals(meta);
        }

        /// <summary>The get hash code.</summary>
        /// <returns>
        ///     The <see cref="int" />.
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = InterfaceType.GetHashCode();
                hashCode = (hashCode * 397) ^ (ContractName?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ Lifetime.GetHashCode();
                return hashCode;
            }
        }

        /// <summary>The to string.</summary>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        public override string ToString()
        {
            object name;
            if (!Metadata.TryGetValue("DebugName", out name)) name = ContractName;
            return ErrorTracer.FormatExport(InterfaceType, name);
        }

        #endregion
    }
}