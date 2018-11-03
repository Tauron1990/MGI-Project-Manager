#region

using System;
using JetBrains.Annotations;
using Tauron.Application.Ioc;

#endregion

namespace Tauron.Application
{
    /// <summary>The command line command base.</summary>
    public abstract class CommandLineCommandBase : ICommandLineCommand
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="CommandLineCommandBase" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="CommandLineCommandBase" /> Klasse.
        ///     Initializes a new instance of the <see cref="CommandLineCommandBase" /> class.
        /// </summary>
        /// <param name="comandName">
        ///     The comand name.
        /// </param>
        protected CommandLineCommandBase([NotNull] string comandName)
        {
            if (string.IsNullOrWhiteSpace(comandName)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(comandName));
            CommandName = comandName;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The execute.
        /// </summary>
        /// <param name="args">
        ///     The args.
        /// </param>
        /// <param name="container">
        ///     The container.
        /// </param>
        public virtual void Execute([NotNull] string[] args, [NotNull] IContainer container)
        {
            CommonConstants.LogCommon(false, "Command: {0} Empty Executet", CommandName);
        }

        #endregion

        #region Public Properties

        /// <summary>Gets the command name.</summary>
        /// <value>The command name.</value>
        [NotNull]
        public string CommandName { get; private set; }

        /// <summary>Gets or sets the factory.</summary>
        /// <value>The factory.</value>
        [CanBeNull]
        public IShellFactory Factory { get; protected set; }

        #endregion
    }
}