#region

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommandLineService.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The command line service.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using Tauron.Application.Ioc;

#endregion

namespace Tauron.Application.Implement
{
    /// <summary>The command line service.</summary>
    [Export(typeof(ICommandLineService))]
    public class CommandLineService : ICommandLineService
    {
        #region Fields

        /// <summary>The _commands.</summary>
        [Inject] private readonly List<ICommandLineCommand> _commands = new List<ICommandLineCommand>();

        #endregion

        #region Public Properties

        /// <summary>Gets the commands.</summary>
        /// <value>The commands.</value>
        public IEnumerable<ICommandLineCommand> Commands => _commands.AsReadOnly();

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The add.
        /// </summary>
        /// <param name="command">
        ///     The command.
        /// </param>
        public void Add(ICommandLineCommand command)
        {
            _commands.Add(command);
        }

        #endregion
    }
}