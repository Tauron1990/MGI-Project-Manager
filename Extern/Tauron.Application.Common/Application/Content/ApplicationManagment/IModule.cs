#region

using JetBrains.Annotations;

#endregion

namespace Tauron.Application
{
    /// <summary>The Module interface.</summary>
    public interface IModule
    {
        int Order { get; }

        #region Public Methods and Operators

        /// <summary>
        ///     The initialize.
        /// </summary>
        /// <param name="application">
        ///     The application.
        /// </param>
        void Initialize([NotNull] CommonApplication application);

        #endregion
    }
}