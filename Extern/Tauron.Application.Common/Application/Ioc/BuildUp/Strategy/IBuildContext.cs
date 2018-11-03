#region

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IBuildContext.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The BuildContext interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using JetBrains.Annotations;
using Tauron.Application.Ioc.BuildUp.Exports;

#endregion

namespace Tauron.Application.Ioc.BuildUp.Strategy
{
    /// <summary>The BuildContext interface.</summary>
    [PublicAPI]
    public interface IBuildContext
    {
        #region Public Properties

        IResolverExtension[] ResolverExtensions { get; }

        /// <summary>Gets or sets a value indicating whether build compled.</summary>
        /// <value>The build compled.</value>
        bool BuildCompled { get; set; }

        /// <summary>Gets the container.</summary>
        /// <value>The container.</value>
        [NotNull]
        IContainer Container { get; }

        /// <summary>Gets or sets the export type.</summary>
        /// <value>The export type.</value>
        [NotNull]
        Type ExportType { get; set; }

        /// <summary>Gets the metadata.</summary>
        /// <value>The metadata.</value>
        [NotNull]
        ExportMetadata Metadata { get; }

        /// <summary>Gets the mode.</summary>
        /// <value>The mode.</value>
        BuildMode Mode { get; }

        /// <summary>Gets the policys.</summary>
        /// <value>The policys.</value>
        [NotNull]
        PolicyList Policys { get; }

        /// <summary>Gets or sets the target.</summary>
        /// <value>The target.</value>
        [CanBeNull]
        object Target { get; set; }

        [NotNull] ErrorTracer ErrorTracer { get; }

        [CanBeNull] BuildParameter[] Parameters { get; }

        #endregion
    }
}