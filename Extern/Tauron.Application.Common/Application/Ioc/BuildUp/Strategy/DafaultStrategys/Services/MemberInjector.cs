#region

using JetBrains.Annotations;
using Tauron.Application.Ioc.BuildUp.Exports;

#endregion

namespace Tauron.Application.Ioc.BuildUp.Strategy.DafaultStrategys
{
    public abstract class MemberInjector
    {
        #region Public Methods and Operators

        /// <summary>
        ///     The inject.
        /// </summary>
        /// <param name="target">
        ///     The target.
        /// </param>
        /// <param name="container">
        ///     The container.
        /// </param>
        /// <param name="metadata">
        ///     The metadata.
        /// </param>
        /// <param name="interceptor"></param>
        /// <param name="errorTracer"></param>
        /// <param name="parameters"></param>
        public abstract void Inject([NotNull] object target, [NotNull] IContainer container, [NotNull] ImportMetadata metadata, [CanBeNull] IImportInterceptor interceptor,
            [NotNull] ErrorTracer errorTracer,
            [CanBeNull] BuildParameter[] parameters);

        #endregion
    }
}