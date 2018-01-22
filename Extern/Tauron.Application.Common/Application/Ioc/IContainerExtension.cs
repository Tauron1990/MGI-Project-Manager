#region

using JetBrains.Annotations;
using Tauron.Application.Ioc.Components;

#endregion

namespace Tauron.Application.Ioc
{
    /// <summary>The ContainerExtension interface.</summary>
    [PublicAPI]
    public interface IContainerExtension
    {
        #region Public Methods and Operators

        /// <summary>
        ///     The initialize.
        /// </summary>
        /// <param name="components">
        ///     The components.
        /// </param>
        void Initialize([NotNull] ComponentRegistry components);

        #endregion
    }
}