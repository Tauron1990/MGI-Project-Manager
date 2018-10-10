#region

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICache.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The Cache interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Tauron.Application.Ioc.BuildUp.Exports;
using Tauron.Application.Ioc.LifeTime;

#endregion

namespace Tauron.Application.Ioc.BuildUp.Strategy
{
    /// <summary>The Cache interface.</summary>
    public interface ICache
    {
        #region Public Methods and Operators

        /// <summary>
        ///     The add.
        /// </summary>
        /// <param name="context">
        ///     The context.
        /// </param>
        /// <param name="metadata">
        ///     The metadata.
        /// </param>
        /// <param name="shareLifetime">
        ///     The share lifetime.
        /// </param>
        void Add(ILifetimeContext context, ExportMetadata metadata, bool shareLifetime);

        /// <summary>
        ///     The get.
        /// </summary>
        /// <param name="metadata">
        ///     The metadata.
        /// </param>
        /// <returns>
        ///     The <see cref="ILifetimeContext" />.
        /// </returns>
        ILifetimeContext GetContext(ExportMetadata metadata);

        #endregion
    }
}