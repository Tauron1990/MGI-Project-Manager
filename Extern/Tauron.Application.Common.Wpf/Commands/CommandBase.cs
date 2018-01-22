#region

using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Input;
using JetBrains.Annotations;

#endregion

namespace Tauron.Application.Commands
{
    public abstract class CommandBase : ICommand
    {
        #region Public Events

        /// <summary>
        ///     Tritt ein, wenn Änderungen auftreten, die sich auf die Ausführung des Befehls auswirken.
        /// </summary>
        public virtual event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;

            remove => CommandManager.RequerySuggested -= value;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Definiert die Methode, mit der ermittelt wird, ob der Befehl im aktuellen Zustand ausgeführt werden kann.
        /// </summary>
        /// <returns>
        ///     true, wenn der Befehl ausgeführt werden kann, andernfalls false.
        /// </returns>
        /// <param name="parameter">
        ///     Daten, die vom Befehl verwendet werden.Wenn der Befehl keine Datenübergabe erfordert, kann das Objekt auf null
        ///     festgelegt werden.
        /// </param>
        public virtual bool CanExecute([CanBeNull] object parameter)
        {
            return true;
        }

        /// <summary>
        ///     Definiert die Methode, die aufgerufen werden soll, wenn der Befehl aufgerufen wird.
        /// </summary>
        /// <param name="parameter">
        ///     Daten, die vom Befehl verwendet werden.Wenn der Befehl keine Datenübergabe erfordert, kann das Objekt auf null
        ///     festgelegt werden.
        /// </param>
        public abstract void Execute([CanBeNull] object parameter);

        /// <summary>
        ///     Ruft das Event CanExecuteChanged auf.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate")]
        [UsedImplicitly]
        // ReSharper disable VirtualMemberNeverOverriden.Global
        public virtual void RaiseCanExecuteChanged()
        {
            // ReSharper restore VirtualMemberNeverOverriden.Global
            CommandManager.InvalidateRequerySuggested();
        }

        #endregion
    }
}