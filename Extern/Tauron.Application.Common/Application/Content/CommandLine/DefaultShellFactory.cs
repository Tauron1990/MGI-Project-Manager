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
            if (shellType == null) throw new ArgumentNullException(nameof(shellType));
            _shellType = shellType;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>The create view.</summary>
        /// <returns>
        ///     The <see cref="object" />.
        /// </returns>
        [NotNull]
        public object CreateView()
        {
            return Activator.CreateInstance(_shellType);
        }

        #endregion
    }
}