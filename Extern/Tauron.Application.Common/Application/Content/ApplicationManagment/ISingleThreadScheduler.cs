#region

using System;
using JetBrains.Annotations;

#endregion

namespace Tauron.Application
{
    /// <summary>The SingleThreadScheduler interface.</summary>
    [PublicAPI]
    public interface ISingleThreadScheduler
    {
        #region Public Properties

        /// <summary>Gets or sets a value indicating whether is background.</summary>
        /// <value>The is background.</value>
        bool IsBackground { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The queue.
        /// </summary>
        /// <param name="task">
        ///     The task.
        /// </param>
        void Queue([NotNull] Action task);

        #endregion
    }
}