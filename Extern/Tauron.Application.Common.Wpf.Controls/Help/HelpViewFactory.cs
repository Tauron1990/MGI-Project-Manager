// The file HelpViewFactory.cs is part of Tauron.Application.Common.Wpf.Controls.
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
//  along with Tauron.Application.Common.Wpf.Controls If not, see <http://www.gnu.org/licenses/>.

#region

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HelpViewFactory.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The help view factory.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Windows;
using JetBrains.Annotations;
using Tauron.Application.Help.View;
using Tauron.Application.Help.ViewModels;

#endregion

namespace Tauron.Application.Help
{
    /// <summary>The help view factory.</summary>
    [PublicAPI]
    public static class HelpViewFactory
    {
        #region Static Fields

        private static Window _windowRef;

        #endregion

        #region Public Methods and Operators

        /// <summary>The get help view.</summary>
        /// <returns>
        ///     The <see cref="Window" />.
        /// </returns>
        [NotNull]
        public static Window GetHelpView()
        {
            return new HelpView();
        }

        /// <summary>
        ///     The get help view model.
        /// </summary>
        /// <param name="filePath">
        ///     The file path.
        /// </param>
        /// <param name="topic">
        ///     The topic.
        /// </param>
        /// <param name="group">
        ///     The group.
        /// </param>
        /// <returns>
        ///     The <see cref="HelpViewModel" />.
        /// </returns>
        [NotNull]
        public static HelpViewModel GetHelpViewModel([NotNull] string filePath, [CanBeNull] string topic,
            [CanBeNull] string group)
        {
            if (filePath == null) throw new ArgumentNullException(nameof(filePath));

            var temp = new HelpViewModel(filePath);
            temp.Activate(topic, group);
            return temp;
        }

        /// <summary>
        ///     The show help.
        /// </summary>
        /// <param name="filePath">
        ///     The file path.
        /// </param>
        /// <param name="topic">
        ///     The topic.
        /// </param>
        /// <param name="group">
        ///     The group.
        /// </param>
        public static void ShowHelp([NotNull] string filePath, [CanBeNull] string topic, [CanBeNull] string group)
        {
            if (filePath == null) throw new ArgumentNullException(nameof(filePath));

            var window = _windowRef;

            window?.Activate();

            window = GetHelpView();
            window.Closed += (sender, e) => _windowRef = null;
            window.DataContext = GetHelpViewModel(filePath, topic, group);
            window.Show();
            _windowRef = window;
        }

        /// <summary>
        ///     The show help lock.
        /// </summary>
        /// <param name="filePath">
        ///     The file path.
        /// </param>
        /// <param name="topic">
        ///     The topic.
        /// </param>
        /// <param name="group">
        ///     The group.
        /// </param>
        public static void ShowHelpLock([NotNull] string filePath, [CanBeNull] string topic, [CanBeNull] string group)
        {
            if (filePath == null) throw new ArgumentNullException(nameof(filePath));

            var window = GetHelpView();
            window.DataContext = GetHelpViewModel(filePath, topic, group);
            window.ShowDialog();
        }

        #endregion
    }
}