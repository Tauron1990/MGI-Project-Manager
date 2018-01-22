#region

using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Input;
using JetBrains.Annotations;

#endregion

namespace Tauron.Application
{
    /// <summary>The TabWorkspace interface.</summary>
    [PublicAPI]
    public interface ITabWorkspace
    {
        #region Public Events

        /// <summary>The close.</summary>
        [SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly")]
        event Action<ITabWorkspace> Close;

        #endregion

        #region Public Properties

        /// <summary>Gets or sets a value indicating whether can close.</summary>
        bool CanClose { get; set; }

        /// <summary>Gets the close workspace.</summary>
        [NotNull]
        ICommand CloseWorkspace { get; }

        /// <summary>Gets or sets the title.</summary>
        [NotNull]
        string Title { get; set; }

        #endregion

        #region Public Methods and Operators

        void OnClose();

        void OnActivate();

        void OnDeactivate();

        #endregion
    }
}