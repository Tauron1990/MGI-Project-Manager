// The file PartManager.cs is part of Tauron.Application.Common.
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
// <copyright file="PartManager.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The build object.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Tauron.Application.Ioc.BuildUp.Exports;

#endregion

namespace Tauron.Application.Ioc.Components
{
    /// <summary>The build object.</summary>
    [PublicAPI]
    public class BuildObject : IWeakReference
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initialisiert eine neue Instanz der <see cref="BuildObject" /> Klasse.
        /// </summary>
        /// <param name="imports">
        ///     The imports.
        /// </param>
        /// <param name="targetExport">
        /// </param>
        public BuildObject([NotNull] IEnumerable<ImportMetadata> imports, [NotNull] ExportMetadata targetExport, [CanBeNull] BuildParameter[] buildParameters)
        {
            if (imports == null) throw new ArgumentNullException(nameof(imports));
            if (targetExport == null) throw new ArgumentNullException(nameof(targetExport));
            Metadata = targetExport;
            this.imports = imports.ToArray();
            Export = targetExport.Export;
            BuildParameters = buildParameters;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>The get imports.</summary>
        /// <returns>
        ///     The <see cref="ImportMetadata[]" />.
        /// </returns>
        public ImportMetadata[] GetImports()
        {
            return (ImportMetadata[]) imports.Clone();
        }

        #endregion

        #region Fields

        private readonly ImportMetadata[] imports;

        /// <summary>The _instance.</summary>
        private WeakReference _instance;

        #endregion

        #region Public Properties

        /// <summary>Gets or sets the export.</summary>
        /// <value>The export.</value>
        public IExport Export { get; private set; }

        /// <summary>Gets or sets the instance.</summary>
        /// <value>The instance.</value>
        public object Instance
        {
            get => _instance.Target;

            set => _instance = new WeakReference(value);
        }

        /// <summary>Gets or sets the metadata.</summary>
        public ExportMetadata Metadata { get; set; }

        /// <summary>Gets a value indicating whether is alive.</summary>
        /// <value>The is alive.</value>
        public bool IsAlive => _instance.IsAlive;

        [CanBeNull]
        public BuildParameter[] BuildParameters { get; set; }

        #endregion
    }

    /// <summary>The rebuild manager.</summary>
    [PublicAPI]
    public class RebuildManager
    {
        #region Fields

        /// <summary>The _objects.</summary>
        private readonly GroupDictionary<ExportMetadata, BuildObject> _objects =
            new GroupDictionary<ExportMetadata, BuildObject>(typeof(WeakReferenceCollection<BuildObject>));

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The add build.
        /// </summary>
        /// <param name="instance">
        ///     The instance.
        /// </param>
        public void AddBuild([NotNull] BuildObject instance)
        {
            if (instance == null) throw new ArgumentNullException(nameof(instance));
            lock (this)
            {
                _objects[instance.Metadata].Add(instance);
            }
        }

        /// <summary>
        ///     The get affected parts.
        /// </summary>
        /// <param name="added">
        ///     The added.
        /// </param>
        /// <param name="removed">
        ///     The removed.
        /// </param>
        /// <returns>
        ///     The <see cref="IEnumerable" />.
        /// </returns>
        public IEnumerable<BuildObject> GetAffectedParts(
            [NotNull] IEnumerable<ExportMetadata> added,
            [NotNull] IEnumerable<ExportMetadata> removed)
        {
            if (added == null) throw new ArgumentNullException(nameof(added));
            if (removed == null) throw new ArgumentNullException(nameof(removed));
            lock (this)
            {
                var changed = added.Concat(removed);

                return from o in _objects
                    from buildObject in o.Value
                    where
                        buildObject.GetImports()
                            .Any(
                                tup =>
                                    changed.Any(
                                        meta =>
                                            tup.InterfaceType == meta.InterfaceType
                                            && tup.ContractName == meta.ContractName))
                    where buildObject.IsAlive
                    select buildObject;
            }
        }

        #endregion
    }
}