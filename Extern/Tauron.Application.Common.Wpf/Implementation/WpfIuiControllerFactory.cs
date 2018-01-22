// The file WpfIuiControllerFactory.cs is part of Tauron.Application.Common.Wpf.
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
// <copyright file="WpfIuiControllerFactory.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The wpf iui controller factory.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Tauron.Application.Ioc;

#endregion

namespace Tauron.Application.Implementation
{
    #region

    using App = System.Windows.Application;

    #endregion

    /// <summary>The wpf iui controller factory.</summary>
    [Export(typeof(IUIControllerFactory))]
    public class WpfIuiControllerFactory : IUIControllerFactory
    {
        private readonly App _app;

        public WpfIuiControllerFactory()
        {
            
        }

        public WpfIuiControllerFactory(App app)
        {
            _app = app;
        }

        #region Explicit Interface Methods

        void IUIControllerFactory.SetSynchronizationContext()
        {
            SetSynchronizationContext();
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>The create controller.</summary>
        /// <returns>
        ///     The <see cref="IUIController" />.
        /// </returns>
        public IUIController CreateController()
        {
            return new WpfApplicationController(_app);
        }

        /// <summary>The set synchronization context.</summary>
        public static void SetSynchronizationContext()
        {
            UiSynchronize.Synchronize = new WPFSynchronize(App.Current.Dispatcher);
        }

        #endregion
    }
}