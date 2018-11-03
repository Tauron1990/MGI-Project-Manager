// The file BuildEngine.cs is part of Tauron.Application.Common.
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
// <copyright file="BuildEngine.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The build engine.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Tauron.Application.Ioc.BuildUp.Exports;
using Tauron.Application.Ioc.BuildUp.Exports.DefaultExports;
using Tauron.Application.Ioc.BuildUp.Strategy;
using Tauron.Application.Ioc.Components;

#endregion

namespace Tauron.Application.Ioc.BuildUp
{
    /// <summary>The build engine.</summary>
    [PublicAPI]
    public sealed class BuildEngine
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="BuildEngine" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="BuildEngine" /> Klasse.
        ///     Initializes a new instance of the <see cref="BuildEngine" /> class.
        /// </summary>
        /// <param name="container">
        ///     The container.
        /// </param>
        /// <param name="providerRegistry">
        ///     The provider registry.
        /// </param>
        /// <param name="componentRegistry">
        ///     The component registry.
        /// </param>
        public BuildEngine(
            [NotNull] IContainer container,
            [NotNull] ExportProviderRegistry providerRegistry,
            [NotNull] ComponentRegistry componentRegistry)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));
            if (providerRegistry == null) throw new ArgumentNullException(nameof(providerRegistry));
            if (componentRegistry == null) throw new ArgumentNullException(nameof(componentRegistry));
            _container = container;
            _componentRegistry = componentRegistry;
            _factory =
                componentRegistry.GetAll<IExportFactory>()
                    .First(fac => fac.TechnologyName == AopConstants.DefaultExportFactoryName)
                    .CastObj<DefaultExportFactory>();
            Pipeline = new Pipeline(componentRegistry);
            RebuildManager = new RebuildManager();
            providerRegistry.ExportsChanged += ExportsChanged;
        }

        #endregion

        #region Fields

        /// <summary>The _factory.</summary>
        private readonly DefaultExportFactory _factory;

        /// <summary>The _container.</summary>
        private readonly IContainer _container;

        [NotNull] private readonly ComponentRegistry _componentRegistry;

        #endregion

        #region Public Properties

        /// <summary>Gets the pipeline.</summary>
        /// <value>The pipeline.</value>
        public Pipeline Pipeline { get; }

        /// <summary>Gets the rebuild manager.</summary>
        /// <value>The rebuild manager.</value>
        public RebuildManager RebuildManager { get; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The build up.
        /// </summary>
        /// <param name="export">
        ///     The export.
        /// </param>
        /// <param name="contractName">
        ///     The contract name.
        /// </param>
        /// <param name="tracer"></param>
        /// <param name="buildParameters"></param>
        /// <returns>
        ///     The <see cref="object" />.
        /// </returns>
        public object BuildUp([NotNull] IExport export, [CanBeNull] string contractName, [NotNull] ErrorTracer tracer,
            [CanBeNull] BuildParameter[] buildParameters)
        {
            if (export == null) throw new ArgumentNullException(nameof(export));
            if (tracer == null) throw new ArgumentNullException(nameof(tracer));
            lock (export)
            {
                try
                {
                    tracer.Phase = "Begin Building Up";
                    var context = new DefaultBuildContext(export, BuildMode.Resolve, _container, contractName,
                        tracer, buildParameters, _componentRegistry.GetAll<IResolverExtension>().ToArray());
                    var buildObject = new BuildObject(export.ImportMetadata, context.Metadata, buildParameters);
                    Pipeline.Build(context);
                    if (tracer.Exceptional) return null;
                    buildObject.Instance = context.Target;
                    if (!export.ExternalInfo.External && !export.ExternalInfo.HandlesLiftime)
                        RebuildManager.AddBuild(buildObject);

                    return context.Target;
                }
                catch (Exception e)
                {
                    tracer.Exceptional = true;
                    tracer.Exception = e;
                    return null;
                }
            }
        }

        /// <summary>
        ///     The build up.
        /// </summary>
        /// <param name="toBuild"></param>
        /// <param name="export">
        ///     The export.
        /// </param>
        /// <param name="errorTracer"></param>
        /// <returns>
        ///     The <see cref="object" />.
        /// </returns>
        public object BuildUp([NotNull] object toBuild, [NotNull] ErrorTracer errorTracer,
            [NotNull] BuildParameter[] buildParameters)
        {
            if (toBuild == null) throw new ArgumentNullException(nameof(toBuild));
            if (errorTracer == null) throw new ArgumentNullException(nameof(errorTracer));
            if (buildParameters == null) throw new ArgumentNullException(nameof(buildParameters));
            lock (toBuild)
            {
                try
                {
                    errorTracer.Phase = "Begin Building Up";
                    var context = new DefaultBuildContext(
                        _factory.CreateAnonymosWithTarget(toBuild.GetType(), toBuild),
                        BuildMode.BuildUpObject,
                        _container,
                        toBuild.GetType().Name, errorTracer,
                        buildParameters, _componentRegistry.GetAll<IResolverExtension>().ToArray());
                    Pipeline.Build(context);
                    return context.Target;
                }
                catch (Exception e)
                {
                    errorTracer.Exceptional = true;
                    errorTracer.Exception = e;
                    return null;
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        ///     The build up.
        /// </summary>
        /// <param name="type">
        ///     The type.
        /// </param>
        /// <param name="constructorArguments">
        ///     The constructor arguments.
        /// </param>
        /// <param name="errorTracer"></param>
        /// <returns>
        ///     The <see cref="object" />.
        /// </returns>
        internal object BuildUp([NotNull] Type type, [CanBeNull] object[] constructorArguments, ErrorTracer errorTracer,
            [CanBeNull] BuildParameter[] buildParameters)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            lock (type)
            {
                errorTracer.Phase = "Begin Building Up";
                try
                {
                    var context = new DefaultBuildContext(
                        _factory.CreateAnonymos(type, constructorArguments),
                        BuildMode.BuildUpObject,
                        _container,
                        type.Name, errorTracer,
                        buildParameters, _componentRegistry.GetAll<IResolverExtension>().ToArray());
                    Pipeline.Build(context);
                    return context.Target;
                }
                catch (Exception e)
                {
                    errorTracer.Exceptional = true;
                    errorTracer.Exception = e;
                    return null;
                }
            }
        }

        private void BuildUp(BuildObject build, ErrorTracer errorTracer, BuildParameter[] buildParameters)
        {
            lock (build.Export)
            {
                var context = new DefaultBuildContext(build, _container, errorTracer, buildParameters);
                build.Instance = context.Target;
                Pipeline.Build(context);
            }
        }

        /// <summary>
        ///     The exports changed.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="e">
        ///     The e.
        /// </param>
        private void ExportsChanged([NotNull] object sender, [NotNull] ExportChangedEventArgs e)
        {
            if (sender == null) throw new ArgumentNullException(nameof(sender));
            if (e == null) throw new ArgumentNullException(nameof(e));

            var parts = RebuildManager.GetAffectedParts(e.Added, e.Removed);

            var errors = new List<ErrorTracer>();

            foreach (var buildObject in parts)
            {
                var errorTracer = new ErrorTracer();
                BuildUp(buildObject, errorTracer, buildObject.BuildParameters);

                if (errorTracer.Exceptional)
                    errors.Add(errorTracer);
            }

            if (errors.Count != 0)
                throw new AggregateException(errors.Select(err => new BuildUpException(err)));
        }

        #endregion
    }
}