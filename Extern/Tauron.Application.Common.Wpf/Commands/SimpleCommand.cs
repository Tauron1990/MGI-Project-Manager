#region

using System;
using System.Windows.Input;
using JetBrains.Annotations;

#endregion

namespace Tauron.Application.Commands
{
    /// <summary>The simple command.</summary>
    [PublicAPI]
    public class SimpleCommand : ICommand
    {
        #region Public Events

        /// <summary>The can execute changed.</summary>
        public event EventHandler CanExecuteChanged
        {
            add
            {
                if (_canExecute != null) CommandManager.RequerySuggested += value;
            }

            remove
            {
                if (_canExecute != null) CommandManager.RequerySuggested -= value;
            }
        }

        #endregion

        #region Fields

        private readonly Func<object, bool> _canExecute;

        private readonly Action<object> _execute;
        [CanBeNull] private readonly object _parameter;

        #endregion

        #region Constructors and Destructors

        public SimpleCommand([CanBeNull] Func<object, bool> canExecute, [NotNull] Action<object> execute)
            : this(canExecute, execute, null)
        {
        }

        public SimpleCommand([CanBeNull] Func<object, bool> canExecute, [NotNull] Action<object> execute, [CanBeNull] object parameter)
        {
            _canExecute = canExecute;
            _execute = execute;
            _parameter = parameter;
        }

        public SimpleCommand([NotNull] Action<object> execute)
            : this(null, execute)
        {
        }

        #endregion

        #region Public Methods and Operators

        public bool CanExecute([CanBeNull] object parameter)
        {
            if (_parameter == null)
                parameter = _parameter;

            return _canExecute == null || _canExecute(parameter);
        }

        /// <summary>
        ///     The execute.
        /// </summary>
        /// <param name="parameter">
        ///     The parameter.
        /// </param>
        public void Execute([CanBeNull] object parameter)
        {
            if (_parameter == null)
                parameter = _parameter;
            if (_execute != null) _execute(parameter);
        }

        #endregion
    }
}