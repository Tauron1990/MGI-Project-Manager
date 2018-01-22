#region

using JetBrains.Annotations;
using Tauron.Application.Ioc;

#endregion

namespace Tauron.Application.Composition
{
    public abstract class XamlCatalog
    {
        #region Methods

        /// <summary>
        ///     The fill container.
        /// </summary>
        /// <param name="container">
        ///     The container.
        /// </param>
        protected internal abstract void FillContainer([NotNull] ExportResolver container);

        #endregion
    }
}