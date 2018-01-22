// The file WpfApplicationController.cs is part of Tauron.Application.Common.Wpf.
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
using System.Globalization;
using System.Threading;
using System.Windows;
using JetBrains.Annotations;

#endregion

namespace Tauron.Application.Implementation
{
    internal class WpfApplicationController : IUIController
    {
        #region Static Fields

        private static ManualResetEventSlim _waiter;

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The run.
        /// </summary>
        /// <param name="window">
        ///     The window.
        /// </param>
        public void Run(IWindow window)
        {
            var win = window as WpfWindow;
            Application.Dispatcher.BeginInvoke(
                win == null
                    ? new Action(() => Application.Run())
                    : (() => Application.Run((Window) win.TranslateForTechnology())));
        }

        #endregion

        #region Explicit Interface Methods

        void IUIController.Shutdown()
        {
            Shutdown();
        }

        #endregion

        #region Public Properties

        /// <summary>Gets or sets the main window.</summary>
        /// <exception cref="InvalidOperationException"></exception>
        public IWindow MainWindow
        {
            get
            {
                var win = Application.MainWindow;
                return win == null ? null : new WpfWindow(win);
            }

            set
            {
                if (value == null) Application.MainWindow = null;

                var wpfwindow = value as WpfWindow;
                if (wpfwindow == null) throw new InvalidOperationException();

                Application.MainWindow = (Window) wpfwindow.TranslateForTechnology();
            }
        }

        /// <summary>Gets or sets the shutdown mode.</summary>
        public ShutdownMode ShutdownMode
        {
            get => (ShutdownMode) Application.ShutdownMode;

            set => Application.ShutdownMode = (System.Windows.ShutdownMode) value;
        }

        #endregion

        #region Properties

        private static object _waiterLock = new object();

        public WpfApplicationController(System.Windows.Application app)
        {
            Application = app;
        }

        internal static System.Windows.Application Application { get; private set; }

        #endregion

        #region Methods

        internal static void Initialize([CanBeNull] CultureInfo info)
        {
            if(Application != null) return;

            _waiter = new ManualResetEventSlim();
            var runner = new Thread(RunApplication) {IsBackground = false};

            if (info != null && !info.Equals(CultureInfo.InvariantCulture))
            {
                runner.CurrentCulture = info;
                runner.CurrentUICulture = info;
            }

            runner.SetApartmentState(ApartmentState.STA);
            lock (_waiterLock)
            {
                runner.Start();
                _waiter.Wait();
            }
        }

        internal static void Shutdown()
        {
            Application?.Dispatcher?.Invoke(Application.Shutdown);
        }

        private static void RunApplication()
        {
            Application = new System.Windows.Application {ShutdownMode = System.Windows.ShutdownMode.OnExplicitShutdown};

            WpfIuiControllerFactory.SetSynchronizationContext();
            _waiter.Set();
            lock (_waiterLock)
            {
                _waiter.Dispose();
                _waiter = null;
            }
            Application.Run();

            Debug.Print("WPF Application Exited");
        }

        #endregion
    }
}