using System;
using JetBrains.Annotations;

namespace Tauron.Application.Wpf.Commands
{
    [PublicAPI]
    public sealed class EventCommand : CommandBase
    {
        public event Func<object?, bool>? CanExecuteEvent;

        public event Action<object?>? ExecuteEvent;

        public override bool CanExecute(object parameter) => OnCanExecute(parameter);

        public override void Execute(object parameter)
        {
            OnExecute(parameter);
        }

        private bool OnCanExecute(object? parameter)
        {
            var handler = CanExecuteEvent;
            return handler == null || handler(parameter);
        }

        private void OnExecute(object? parameter)
        {
            ExecuteEvent?.Invoke(parameter);
        }
    }
}