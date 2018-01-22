// The file DefaultExport.cs is part of Tauron.Application.Common.
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
// <copyright file="DefaultExport.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The default export.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using Tauron.Application.Ioc.LifeTime;

#endregion

namespace Tauron.Application.Ioc.BuildUp.Exports.DefaultExports
{
    /// <summary>The default export.</summary>
    public sealed class DefaultExport : IExport
    {
        #region Methods

        /// <summary>
        ///     The initialize.
        /// </summary>
        /// <param name="anonym">
        ///     The anonym.
        /// </param>
        private void Initialize(bool anonym)
        {
            _isAnonymos = anonym;

            Globalmetadata = new Dictionary<string, object>();

            IEnumerable<ExportMetadataBaseAttribute> metadata =
                _attributeProvider.GetAllCustomAttributes<ExportMetadataBaseAttribute>();
            foreach (var exportMetadataAttribute in metadata) Globalmetadata[exportMetadataAttribute.InternalKey] = exportMetadataAttribute.InternalValue;

            var attr = Globalmetadata.TryGetAndCast<LifetimeContextAttribute>(AopConstants.LiftimeMetadataName);

            var lifetime = attr?.LifeTimeType ?? typeof(SharedLifetime);

            if (anonym)
            {
                _exports = new[]
                {
                    new ExportMetadata(
                        _exportetType,
                        ExternalInfo.ExtenalComponentName,
                        typeof(NotSharedLifetime),
                        new Dictionary<string, object>(Globalmetadata),
                        this)
                };
                return;
            }

            _exports = _attributeProvider.GetAllCustomAttributes<ExportAttribute>()
                .Select(
                    attribute =>
                    {
                        var temp = new Dictionary<string, object>(Globalmetadata);

                        Type realLifetime;

                        if (attr == null)
                        {
                            var customLifeTime = attribute.GetOverrideDefaultPolicy();
                            if (customLifeTime != null)
                            {
                                realLifetime = customLifeTime.LifeTimeType;
                                attr = customLifeTime;
                            }
                            else
                            {
                                realLifetime = lifetime;
                            }
                        }
                        else
                        {
                            realLifetime = lifetime;
                        }

                        foreach (var tuple in attribute.Metadata)
                            temp.Add(tuple.Item1,
                                tuple.Item2);

                        return new ExportMetadata(attribute.Export, attribute.ContractName, realLifetime, temp, this);
                    })
                .ToArray();

            ShareLifetime = attr == null || attr.ShareLiftime;
        }

        #endregion

        #region Fields

        private readonly ICustomAttributeProvider _attributeProvider;

        /// <summary>The _exportet type.</summary>
        private readonly Type _exportetType;

        /// <summary>The _exports.</summary>
        private ExportMetadata[] _exports;

        private bool _isAnonymos;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="DefaultExport" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="DefaultExport" /> Klasse.
        ///     Initializes a new instance of the <see cref="DefaultExport" /> class.
        /// </summary>
        /// <param name="exportetType">
        ///     The exportet type.
        /// </param>
        /// <param name="externalInfo">
        ///     The external info.
        /// </param>
        /// <param name="asAnonym">
        ///     The as anonym.
        /// </param>
        public DefaultExport([NotNull] Type exportetType, [NotNull] ExternalExportInfo externalInfo, bool asAnonym)
        {
            if (exportetType == null) throw new ArgumentNullException(nameof(exportetType));
            if (externalInfo == null) throw new ArgumentNullException(nameof(externalInfo));

            Globalmetadata = new Dictionary<string, object>();
            _exportetType = exportetType;
            _attributeProvider = exportetType;
            ExternalInfo = externalInfo;
            Initialize(asAnonym);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="DefaultExport" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="DefaultExport" /> Klasse.
        ///     Initializes a new instance of the <see cref="DefaultExport" /> class.
        /// </summary>
        /// <param name="info">
        ///     The exportet type.
        /// </param>
        /// <param name="externalInfo">
        ///     The external info.
        /// </param>
        /// <param name="asAnonym">
        ///     The as anonym.
        /// </param>
        public DefaultExport([NotNull] MethodInfo info, [NotNull] ExternalExportInfo externalInfo, bool asAnonym)
        {
            if (info == null) throw new ArgumentNullException(nameof(info));
            if (externalInfo == null) throw new ArgumentNullException(nameof(externalInfo));
            Globalmetadata = new Dictionary<string, object>();
            _attributeProvider = info;
            ExternalInfo = externalInfo;
            Initialize(asAnonym);
        }

        #endregion

        #region Public Properties

        /// <summary>Gets the export metadata.</summary>
        /// <value>The export metadata.</value>
        [NotNull]
        public IEnumerable<ExportMetadata> ExportMetadata => _exports;

        /// <summary>Gets the exports.</summary>
        /// <value>The exports.</value>
        [NotNull]
        public IEnumerable<Type> Exports
        {
            get { return _exports.Select(ex => ex.InterfaceType); }
        }

        /// <summary>Gets the external info.</summary>
        /// <value>The external info.</value>
        [NotNull]
        public ExternalExportInfo ExternalInfo { get; private set; }

        /// <summary>Gets or sets the globalmetadata.</summary>
        /// <value>The globalmetadata.</value>
        public Dictionary<string, object> Globalmetadata { get; private set; }

        /// <summary>Gets the implement type.</summary>
        /// <value>The implement type.</value>
        public Type ImplementType => _exportetType;

        /// <summary>Gets the import metadata.</summary>
        /// <value>The import metadata.</value>
        public IEnumerable<ImportMetadata> ImportMetadata { get; internal set; }

        /// <summary>Gets a value indicating whether share lifetime.</summary>
        /// <value>The share lifetime.</value>
        public bool ShareLifetime { get; private set; }

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
        public bool Equals(IExport other)
        {
            if (ReferenceEquals(null, other)) return false;

            if (ReferenceEquals(this, other)) return true;

            return ImplementType == other.ImplementType;
        }

        /// <summary>
        ///     The get export metadata.
        /// </summary>
        /// <param name="contractName">
        ///     The contract name.
        /// </param>
        /// <returns>
        ///     The <see cref="ExportMetadata" />.
        /// </returns>
        public ExportMetadata GetNamedExportMetadata(string contractName)
        {
            return _isAnonymos ? _exports[0] : _exports.Single(exm => exm.ContractName == contractName);
        }

        /// <summary>
        ///     The select contract name.
        /// </summary>
        /// <param name="contractName">
        ///     The contract name.
        /// </param>
        /// <returns>
        ///     The <see cref="IEnumerable{T}" />.
        /// </returns>
        public IEnumerable<ExportMetadata> SelectContractName(string contractName)
        {
            return _isAnonymos ? _exports : _exports.Where(meta => meta.ContractName == contractName);
        }

        /// <summary>
        ///     The is export.
        /// </summary>
        /// <param name="type">
        ///     The type.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public static bool IsExport([NotNull] ICustomAttributeProvider type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            return type.GetCustomAttributes(typeof(ExportAttribute), false).Length != 0;
        }

        /// <summary>The ==.</summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns></returns>
        public static bool operator ==(DefaultExport left, DefaultExport right)
        {
            return Equals(left, right);
        }

        /// <summary>The !=.</summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns></returns>
        public static bool operator !=(DefaultExport left, DefaultExport right)
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

            return obj is DefaultExport && Equals((DefaultExport) obj);
        }

        /// <summary>The get hash code.</summary>
        /// <returns>
        ///     The <see cref="int" />.
        /// </returns>
        public override int GetHashCode()
        {
            return _attributeProvider.GetHashCode();
        }

        /// <summary>The to string.</summary>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        public override string ToString()
        {
            return ImplementType?.ToString() ?? string.Empty;
        }

        #endregion
    }
}