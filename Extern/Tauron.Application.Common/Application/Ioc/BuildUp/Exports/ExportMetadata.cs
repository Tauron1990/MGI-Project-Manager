#region

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
            InterfaceType = interfaceType;
            ContractName = contractName;
            Lifetime = lifetime;
            Metadata = metadata ?? throw new ArgumentNullException(nameof(metadata));
            Export = export ?? throw new ArgumentNullException(nameof(export));
        }

        #endregion

        #region Fields

        #endregion

        #region Public Properties

        /// <summary>Gets the contract name.</summary>
        /// <value>The contract name.</value>
        [CanBeNull]
        public string ContractName { get; }

        /// <summary>Gets or sets the export.</summary>
        /// <value>The export.</value>
        [NotNull]
        public IExport Export { get; set; }

        /// <summary>Gets the interface type.</summary>
        /// <value>The interface type.</value>
        [NotNull]
        public Type InterfaceType { get; }

        /// <summary>Gets the lifetime.</summary>
        /// <value>The lifetime.</value>
        [NotNull]
        public Type Lifetime { get; }

        /// <summary>Gets the metadata.</summary>
        /// <value>The metadata.</value>
        [CanBeNull]
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
        public bool Equals(ExportMetadata other)
        {
            if (ReferenceEquals(null, other)) return false;

            if (ReferenceEquals(this, other)) return true;

            return InterfaceType == other.InterfaceType && string.Equals(ContractName, other.ContractName)
                   && Lifetime == other.Lifetime && Export.Equals(other.Export);
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
        public override bool Equals(object obj)
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
            if (Metadata == null || !Metadata.TryGetValue("DebugName", out var name)) name = ContractName;
            return ErrorTracer.FormatExport(InterfaceType, name);
        }

        #endregion
    }
}