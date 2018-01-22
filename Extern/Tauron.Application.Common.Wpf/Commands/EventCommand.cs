#region

using System;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

#endregion

namespace Tauron.Application.Commands
{
    /// <summary>The event command.</summary>
    [PublicAPI]
    public sealed class EventCommand : CommandBase
    {
        #region Public Events

        /// <summary>The can execute event.</summary>
        [SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly")]
        public event Func<object, bool> CanExecuteEvent;

        /// <summary>The execute event.</summary>
        [SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly")]
        public event Action<object> ExecuteEvent;

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The can execute.
        /// </summary>
        /// <param name="parameter">
        ///     The parameter.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public override bool CanExecute(object parameter)
        {
            return OnCanExecute(parameter);
        }

        /// <summary>
        ///     The execute.
        /// </summary>
        /// <param name="parameter">
        ///     The parameter.
        /// </param>
        public override void Execute(object parameter)
        {
            OnExecute(parameter);
        }

        #endregion

        #region Methods

        private bool OnCanExecute([CanBeNull] object parameter)
        {
            var handler = CanExecuteEvent;
            return handler == null || handler(parameter);
        }

        private void OnExecute([CanBeNull] object parameter)
        {
            var handler = ExecuteEvent;
            if (handler != null) handler(parameter);
        }

        #endregion
    }
}