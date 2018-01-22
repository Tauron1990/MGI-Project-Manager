// The file WPFSynchronize.cs is part of Tauron.Application.Common.Wpf.
// 
// CoreEngine is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// CoreEngine is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//  
// You should have received a copy of the GNU General Public License
//  along with Tauron.Application.Common.Wpf If not, see <http://www.gnu.org/licenses/>.

#region

using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Threading;
using JetBrains.Annotations;

#endregion

namespace Tauron.Application.Implementation
{
    /// <summary>The wpf synchronize.</summary>
    [PublicAPI]
    [DebuggerNonUserCode]
    public class WPFSynchronize : IUISynchronize
    {
        #region Fields

        private readonly Dispatcher _dispatcher;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="WPFSynchronize" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="WPFSynchronize" /> Klasse.
        /// </summary>
        /// <param name="dispatcher">
        ///     The dispatcher.
        /// </param>
        public WPFSynchronize([NotNull] Dispatcher dispatcher)
        {
            if (dispatcher == null) throw new ArgumentNullException(nameof(dispatcher));
            _dispatcher = dispatcher;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The begin invoke.
        /// </summary>
        /// <param name="action">
        ///     The action.
        /// </param>
        /// <returns>
        ///     The <see cref="Task" />.
        /// </returns>
        [NotNull]
        public Task BeginInvoke([NotNull] Action action)
        {
            return _dispatcher.BeginInvoke(action).Task;
        }

        /// <summary>
        ///     The invoke.
        /// </summary>
        /// <param name="action">
        ///     The action.
        /// </param>
        public void Invoke([NotNull] Action action)
        {
            _dispatcher.Invoke(action);
        }

        /// <summary>
        ///     The invoke.
        /// </summary>
        /// <param name="action">
        ///     The action.
        /// </param>
        /// <typeparam name="TReturn">
        /// </typeparam>
        /// <returns>
        ///     The <see cref="TReturn" />.
        /// </returns>
        public TReturn Invoke<TReturn>([NotNull] Func<TReturn> action)
        {
            return _dispatcher.Invoke(action);
        }

        #endregion
    }
}