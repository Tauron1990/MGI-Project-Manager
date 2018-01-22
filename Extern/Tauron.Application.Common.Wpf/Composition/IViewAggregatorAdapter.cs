#region

using System.Collections.Generic;
using System.Windows;
using JetBrains.Annotations;

#endregion

namespace Tauron.Application.Composition
{
    /// <summary>The ViewAggregatorAdapter interface.</summary>
    public interface IViewAggregatorAdapter
    {
        #region Public Methods and Operators

        /// <summary>
        ///     The adapt.
        /// </summary>
        /// <param name="dependencyObject">
        ///     The dependency object.
        /// </param>
        void Adapt([NotNull] DependencyObject dependencyObject);

        /// <summary>
        ///     The add views.
        /// </summary>
        /// <param name="views">
        ///     The views.
        /// </param>
        void AddViews([NotNull] IEnumerable<object> views);

        /// <summary>
        ///     The can adapt.
        /// </summary>
        /// <param name="dependencyObject">
        ///     The dependency object.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        bool CanAdapt([NotNull] DependencyObject dependencyObject);

        /// <summary>The release.</summary>
        void Release();

        #endregion
    }
}