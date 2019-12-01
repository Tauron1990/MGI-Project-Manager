using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Input;
using JetBrains.Annotations;

namespace Tauron.Application.Wpf.Commands
{
    public abstract class CommandBase : ICommand
    {
        public virtual event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public virtual bool CanExecute([CanBeNull] object parameter) => true;

        public abstract void Execute([CanBeNull] object parameter);

        [UsedImplicitly]
        public virtual void RaiseCanExecuteChanged() 
            => CommandManager.InvalidateRequerySuggested();
    }
}