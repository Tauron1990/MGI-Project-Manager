#region

using System;
using System.Windows.Media;
using JetBrains.Annotations;

#endregion

namespace Tauron.Application
{
    public interface IImageHelper
    {
        #region Public Methods and Operators

        /// <summary>
        ///     The convert.
        /// </summary>
        /// <param name="target">
        ///     The target.
        /// </param>
        /// <param name="assembly">
        ///     The assembly.
        /// </param>
        /// <returns>
        ///     The <see cref="ImageSource" />.
        /// </returns>
        [CanBeNull]
        ImageSource Convert([NotNull] Uri target, [NotNull] string assembly);

        /// <summary>
        ///     The convert.
        /// </summary>
        /// <param name="uri">
        ///     The uri.
        /// </param>
        /// <param name="assembly">
        ///     The assembly.
        /// </param>
        /// <returns>
        ///     The <see cref="ImageSource" />.
        /// </returns>
        [CanBeNull]
        ImageSource Convert([NotNull] string uri, [NotNull] string assembly);

        #endregion
    }
}