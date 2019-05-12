using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace IISServiceManager.Core
{
    public class AsyncCommand : IAsyncCommand
    {
        public event EventHandler CanExecuteChanged;

        private          bool          _isExecuting;
        private readonly Func<object, Task>    _execute;
        private readonly Func<bool>    _canExecute;
        private readonly IErrorHandler _errorHandler;

        public AsyncCommand(
            Func<object, Task>    execute,
            Func<bool>    canExecute   = null,
            IErrorHandler errorHandler = null)
        {
            _execute      = execute;
            _canExecute   = canExecute;
            _errorHandler = errorHandler;

            CommandManager.RequerySuggested += (sender, args) => RaiseCanExecuteChanged();
        }

        public bool CanExecute() => !_isExecuting && (_canExecute?.Invoke() ?? true);

        public async Task ExecuteAsync(object parameter)
        {
            if (CanExecute())
            {
                try
                {
                    _isExecuting = true;
                    await _execute(parameter);
                }
                finally
                {

                    Application.Current.Dispatcher.Invoke(CommandManager.InvalidateRequerySuggested);
                    _isExecuting = false;
                }
            }

            RaiseCanExecuteChanged();
        }

        private void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);

        #region Explicit implementations

        bool ICommand.CanExecute(object parameter) => CanExecute();

        void ICommand.Execute(object parameter) => ExecuteAsync(parameter).FireAndForgetSafeAsync(_errorHandler);

        #endregion
    }
}