#region

using System;
using System.Reflection;
using JetBrains.Annotations;

#endregion

namespace Tauron.Application.Commands
{
    public sealed class EventData
    {
        #region Constructors and Destructors

        internal EventData([NotNull] object sender, [NotNull] EventArgs eventArgs)
        {
            if (sender == null) throw new ArgumentNullException(nameof(sender));
            Sender    = sender;
            EventArgs = eventArgs;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Die Ereignis Daten
        /// </summary>
        [NotNull]
        public EventArgs EventArgs { get; }

        /// <summary>
        ///     Das Objekt, an das der Ereignishandler angefügt wird.
        /// </summary>
        [NotNull]
        public object Sender { get; }

        #endregion
    }

    /// <summary>The method command.</summary>
    public sealed class MethodCommand : CommandBase
    {
        #region Enums

        private enum MethodType
        {
            /// <summary>The zero.</summary>
            Zero = 0,

            /// <summary>The one.</summary>
            One,

            /// <summary>The two.</summary>
            Two,

            /// <summary>The event args.</summary>
            EventArgs
        }

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initialisiert eine neue Instanz der <see cref="MethodCommand" /> Klasse.
        /// </summary>
        /// <param name="method">
        ///     The method.
        /// </param>
        /// <param name="context">
        ///     The context.
        /// </param>
        /// <param name="sync"></param>
        public MethodCommand([NotNull] MethodInfo method, [NotNull] WeakReference context)
        {
            if (method == null) throw new ArgumentNullException(nameof(method));
            if (context == null) throw new ArgumentNullException(nameof(context));
            _method  = method;
            _context = context;

            _methodType = (MethodType) method.GetParameters().Length;
            if (_methodType != MethodType.One) return;
            if (_method.GetParameters()[0].ParameterType != typeof(EventData)) _methodType = MethodType.EventArgs;
        }

        #endregion

        #region Public Properties

        /// <summary>Gets the context.</summary>
        [CanBeNull]
        public object Context => _context?.Target;

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Definiert die Methode, die aufgerufen werden soll, wenn der Befehl aufgerufen wird.
        /// </summary>
        /// <param name="parameter">
        ///     Daten, die vom Befehl verwendet werden.Wenn der Befehl keine Datenübergabe erfordert, kann das Objekt auf null
        ///     festgelegt werden.
        /// </param>
        public override void Execute(object parameter)
        {
            object[] args;

            var temp = (EventData) parameter;
            switch (_methodType)
            {
                case MethodType.Zero:
                    args = new object[0];
                    break;
                case MethodType.One:
                    args = new object[] {temp};
                    break;
                case MethodType.Two:
                    args = new[] {temp?.Sender, temp?.EventArgs};
                    break;
                case MethodType.EventArgs:
                    args = new object[] {temp?.EventArgs};
                    break;
                default:
                    args = new object[0];
                    break;
            }

            _method.Invoke(Context, args);
        }

        #endregion

        #region Fields

        private readonly WeakReference _context;

        private readonly MethodInfo _method;

        private readonly MethodType _methodType;

        #endregion
    }
}