// The file DefaultBuildContext.cs is part of Tauron.Application.Common.
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
// <copyright file="DefaultBuildContext.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The default build context.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using Tauron.Application.Ioc.BuildUp.Exports;
using Tauron.Application.Ioc.Components;

#endregion

namespace Tauron.Application.Ioc.BuildUp.Strategy
{
    /// <summary>The default build context.</summary>
    public sealed class DefaultBuildContext : IBuildContext
    {
        public override string ToString()
        {
            return Metadata.ToString();
        }

        #region Constructors and Destructors

        /// <summary>
        ///     Initialisiert eine neue Instanz der <see cref="DefaultBuildContext" /> Klasse.
        /// </summary>
        /// <param name="targetExport">
        ///     The target export.
        /// </param>
        /// <param name="mode">
        ///     The mode.
        /// </param>
        /// <param name="container">
        ///     The container.
        /// </param>
        /// <param name="contractName">
        ///     The contract name.
        /// </param>
        /// <param name="errorTracer"></param>
        [SuppressMessage("StyleCop.CSharp.SpacingRules", "SA1027:TabsMustNotBeUsed",
            Justification = "Reviewed. Suppression is OK here.")]
        public DefaultBuildContext([NotNull] IExport targetExport, BuildMode mode, [NotNull] IContainer container,
            [CanBeNull] string contractName,
            [NotNull] ErrorTracer errorTracer, [CanBeNull] BuildParameter[] parameters,
            [CanBeNull] IResolverExtension[] resolverExtensions)
        {
            if (targetExport == null) throw new ArgumentNullException(nameof(targetExport));
            if (container == null) throw new ArgumentNullException(nameof(container));
            if (errorTracer == null) throw new ArgumentNullException(nameof(errorTracer));
            if (!Enum.IsDefined(typeof(BuildMode), mode))
                throw new InvalidEnumArgumentException(nameof(mode), (int) mode, typeof(BuildMode));
            Metadata = targetExport.GetNamedExportMetadata(contractName);
            errorTracer.Export = Metadata.ToString();
            ExportType = targetExport.ImplementType;
            Target = null;
            BuildCompled = false;
            Policys = new PolicyList();
            Mode = mode;
            Container = container;
            ErrorTracer = errorTracer;
            Parameters = parameters;
            ResolverExtensions = resolverExtensions;
        }

        /// <summary>
        ///     Initialisiert eine neue Instanz der <see cref="DefaultBuildContext" /> Klasse.
        /// </summary>
        /// <param name="buildObject">
        ///     The build object.
        /// </param>
        /// <param name="container">
        ///     The container.
        /// </param>
        /// <param name="errorTracer"></param>
        /// <param name="parameters"></param>
        public DefaultBuildContext([NotNull] BuildObject buildObject, [NotNull] IContainer container,
            [NotNull] ErrorTracer errorTracer, [CanBeNull] BuildParameter[] parameters)
        {
            if (buildObject == null) throw new ArgumentNullException(nameof(buildObject));
            if (container == null) throw new ArgumentNullException(nameof(container));
            if (errorTracer == null) throw new ArgumentNullException(nameof(errorTracer));
            Metadata = buildObject.Metadata;
            errorTracer.Export = Metadata.ToString();
            Mode = BuildMode.BuildUpObject;
            Policys = new PolicyList();
            Target = buildObject.Instance;
            ExportType = Metadata.Export.ImplementType;
            BuildCompled = false;
            Container = container;
            ErrorTracer = errorTracer;
            Parameters = parameters;
        }

        #endregion

        #region Public Properties

        /// <summary>Gets or sets a value indicating whether build compled.</summary>
        /// <value>The build compled.</value>
        public bool BuildCompled { get; set; }

        /// <summary>Gets the container.</summary>
        /// <value>The container.</value>
        public IContainer Container { get; private set; }

        /// <summary>Gets or sets the export type.</summary>
        /// <value>The export type.</value>
        public Type ExportType { get; set; }

        /// <summary>Gets the metadata.</summary>
        /// <value>The metadata.</value>
        public ExportMetadata Metadata { get; private set; }

        /// <summary>Gets the mode.</summary>
        /// <value>The mode.</value>
        public BuildMode Mode { get; private set; }

        /// <summary>Gets the policys.</summary>
        /// <value>The policys.</value>
        public PolicyList Policys { get; private set; }

        /// <summary>Gets or sets the target.</summary>
        /// <value>The target.</value>
        public object Target { get; set; }

        public ErrorTracer ErrorTracer { get; private set; }
        public BuildParameter[] Parameters { get; private set; }
        public IResolverExtension[] ResolverExtensions { get; set; }

        #endregion
    }
}