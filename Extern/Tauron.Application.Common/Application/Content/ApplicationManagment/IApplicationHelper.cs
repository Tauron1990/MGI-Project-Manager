#region

using System.Diagnostics.CodeAnalysis;
using System.Threading;
using JetBrains.Annotations;

#endregion

namespace Tauron.Application
{
    public interface IApplicationHelper
    {
        #region Public Methods and Operators

        /// <summary>
        ///     The create ui thread.
        /// </summary>
        /// <param name="start">
        ///     The start.
        /// </param>
        /// <returns>
        ///     The <see cref="Thread" />.
        /// </returns>
        [NotNull]
        [PublicAPI]
        Thread CreateUIThread([NotNull] ThreadStart start);

        /// <summary>The run anonymous application.</summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>
        ///     The <see cref="IWindow" />.
        /// </returns>
        [NotNull]
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        [PublicAPI]
        IWindow RunAnonymousApplication<T>() where T : class, IWindow;

        /// <summary>
        ///     The run anonymous application.
        /// </summary>
        /// <param name="window">
        ///     The window.
        /// </param>
        [PublicAPI]
        void RunAnonymousApplication([NotNull] IWindow window);

        /// <summary>
        ///     The run ui thread.
        /// </summary>
        /// <param name="start">
        ///     The start.
        /// </param>
        [PublicAPI]
        void RunUIThread([NotNull] ThreadStart start);

        #endregion
    }
}