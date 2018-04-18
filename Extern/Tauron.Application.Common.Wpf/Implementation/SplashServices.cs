// The file SplashServices.cs is part of Tauron.Application.Common.Wpf.
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

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SplashServices.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The splash service.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Tauron.Application.Implementation.Controls;

#endregion

namespace Tauron.Application.Implementation
{
    /// <summary>The splash service.</summary>
    public class SplashService : ISplashService
    {
        #region Fields

        private SplashScreen _screen;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="SplashService" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="SplashService" /> Klasse.
        /// </summary>
        public SplashService()
        {
            Listner = new SplashMessageListener();
        }

        #endregion

        #region Public Properties

        /// <summary>Gets the listner.</summary>
        public SplashMessageListener Listner { get; private set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>The close splash.</summary>
        public void CloseSplash()
        {
            var context = UiSynchronize.Synchronize;
            context.Invoke(
                           () =>
                           {
                               if (_screen == null) return;
                               _screen.Close();
                               _screen = null;
                           });
        }

        /// <summary>The show splash.</summary>
        public void ShowSplash()
        {
            var context = UiSynchronize.Synchronize;
            context.Invoke(
                           () =>
                           {
                               _screen = new SplashScreen {DataContext = Listner, Width = Listner.Width, Height = Listner.Height};
                               _screen.Show();
                           });
        }

        #endregion
    }
}