#region

using System;
using JetBrains.Annotations;

#endregion

namespace Tauron.Application
{
    /// <summary>The default shell factory.</summary>
    [PublicAPI]
    public class DefaultShellFactory : IShellFactory
    {
        #region Fields

        /// <summary>The _shell type.</summary>
        private readonly Type _shellType;

        #endregion

        #region Constructors and Destructors

        public DefaultShellFactory([NotNull] Type shellType)
        {
            _shellType = shellType ?? throw new ArgumentNullException(nameof(shellType));
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>The create view.</summary>
        /// <returns>
        ///     The <see cref="object" />.
        /// </returns>
        [NotNull]
        public object CreateView() => Activator.CreateInstance(_shellType);

        #endregion
    }
}