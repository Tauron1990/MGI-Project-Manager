using System;
using JetBrains.Annotations;

namespace Tauron.Application.Wpf.Commands
{
    [PublicAPI]
    public class SimpleCommand : CommandBase
    {
        private readonly Func<object?, bool>? _canExecute;

        private readonly Action<object?> _execute;
        private readonly object? _parameter;

        public SimpleCommand(Func<object?, bool>? canExecute, Action<object?> execute, object? parameter = null)
        {
            _canExecute = canExecute;
            _execute = Argument.NotNull(execute, nameof(execute));
            _parameter = parameter;
        }

        public SimpleCommand(Action<object?> execute)
            : this(null, execute) { }

        public override bool CanExecute(object? parameter)
        {
            if (parameter == null)
                parameter = _parameter;

            return _canExecute == null || _canExecute(parameter);
        }
        
        public override void Execute(object? parameter)
        {
            if (parameter == null)
                parameter = _parameter;
            _execute?.Invoke(parameter);
        }
    }
}