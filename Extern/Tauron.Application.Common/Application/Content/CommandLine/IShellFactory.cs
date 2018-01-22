#region

using JetBrains.Annotations;

#endregion

namespace Tauron.Application
{
    public interface IShellFactory
    {
        #region Public Methods and Operators

        /// <summary>The create view.</summary>
        /// <returns>
        ///     The <see cref="object" />.
        /// </returns>
        [NotNull]
        object CreateView();

        #endregion
    }
}