#region

using System;
using JetBrains.Annotations;

#endregion

namespace Tauron.Application.Ioc.BuildUp.Strategy.DafaultStrategys
{
    /// <summary>The build context extensions.</summary>
    [PublicAPI]
    public static class BuildContextExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        ///     The can cache.
        /// </summary>
        /// <param name="context">
        ///     The context.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public static bool CanCache([NotNull] this IBuildContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            return context.Mode != BuildMode.BuildUpObject || context.Target != null
                                                           || context.Metadata.Export.ExternalInfo.External
                                                           && !context.Metadata.Export.ExternalInfo.HandlesLiftime;
        }

        /// <summary>
        ///     The can handle liftime.
        /// </summary>
        /// <param name="context">
        ///     The context.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public static bool CanHandleLiftime([NotNull] this IBuildContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            return !context.Metadata.Export.ExternalInfo.External
                && !context.Metadata.Export.ExternalInfo.HandlesLiftime;
        }

        /// <summary>
        ///     The can use build up.
        /// </summary>
        /// <param name="context">
        ///     The context.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public static bool CanUseBuildUp([NotNull] this IBuildContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            return !context.Metadata.Export.ExternalInfo.External || context.Metadata.Export.ExternalInfo.CanUseBuildup;
        }

        /// <summary>
        ///     The is build export.
        /// </summary>
        /// <param name="context">
        ///     The context.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public static bool IsBuildExport([NotNull] this IBuildContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            return context.Mode != BuildMode.BuildUpObject;
        }

        /// <summary>
        ///     The is resolving.
        /// </summary>
        /// <param name="context">
        ///     The context.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public static bool IsResolving([NotNull] this IBuildContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            return context.Mode == BuildMode.Resolve;
        }

        /// <summary>
        ///     The use internal instantiation.
        /// </summary>
        /// <param name="context">
        ///     The context.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public static bool UseInternalInstantiation([NotNull] this IBuildContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            return !context.Metadata.Export.ExternalInfo.External;
        }

        #endregion
    }
}