#region

using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;

#endregion

namespace Tauron
{
    /// <summary>
    ///     The read only enumerator.
    /// </summary>
    /// <typeparam name="T">
    /// </typeparam>
    [PublicAPI]
    public class ReadOnlyEnumerator<T> : IEnumerable<T>
    {
        #region Fields

        /// <summary>The _enumerable.</summary>
        private readonly IEnumerable<T> _enumerable;

        #endregion

        #region Constructors and Destructors

        public ReadOnlyEnumerator([NotNull] IEnumerable<T> enumerable)
        {
            if (enumerable == null) throw new ArgumentNullException(nameof(enumerable));
            _enumerable = enumerable;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>The get enumerator.</summary>
        /// <returns>
        ///     The <see cref="IEnumerator" />.
        /// </returns>
        public IEnumerator<T> GetEnumerator()
        {
            return _enumerable.GetEnumerator();
        }

        #endregion

        #region Explicit Interface Methods

        /// <summary>The get enumerator.</summary>
        /// <returns>
        ///     The <see cref="IEnumerator" />.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}