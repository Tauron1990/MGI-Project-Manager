#region

using System;
using System.Threading.Tasks;
using JetBrains.Annotations;

#endregion

// ReSharper disable once CheckNamespace
namespace Tauron.Application
{
    /// <summary>The UISynchronize interface.</summary>
    [PublicAPI]
    public interface IUISynchronize
    {
        #region Public Methods and Operators

        /// <summary>
        ///     The begin invoke.
        /// </summary>
        /// <param name="action">
        ///     The action.
        /// </param>
        /// <returns>
        ///     The <see cref="Task" />.
        /// </returns>
        [NotNull]
        Task BeginInvoke([NotNull] Action action);

        [NotNull]
        Task<TResult> BeginInvoke<TResult>([NotNull] Func<TResult> action);

        /// <summary>
        ///     The invoke.
        /// </summary>
        /// <param name="action">
        ///     The action.
        /// </param>
        void Invoke([NotNull] Action action);

        /// <summary>
        ///     The invoke.
        /// </summary>
        /// <param name="action">
        ///     The action.
        /// </param>
        /// <returns>
        ///     The <see cref="TReturn" />.
        /// </returns>
        TReturn Invoke<TReturn>([NotNull] Func<TReturn> action);

        #endregion
    }

    /// <summary>The ui synchronize.</summary>
    [PublicAPI]
    public static class UiSynchronize
    {
        #region Public Properties

        /// <summary>Gets or sets the synchronize.</summary>
        [NotNull]
        public static IUISynchronize Synchronize { get; set; }

        #endregion
    }
}