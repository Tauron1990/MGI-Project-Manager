#region

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using JetBrains.Annotations;
using Tauron.Application.Ioc;

#endregion

namespace Tauron.Application.Implement
{
    /// <summary>The application helper.</summary>
    [PublicAPI]
    [Export(typeof(IApplicationHelper))]
    public class ApplicationHelper : IApplicationHelper
    {
        /// <summary>
        ///     The start up helper.
        /// </summary>
        /// <typeparam name="T">
        /// </typeparam>
        private class StartUpHelper<T>
            where T : class, IWindow
        {
            #region Constructors and Destructors

            /// <summary>
            ///     Initializes a new instance of the <see cref="StartUpHelper{T}" /> class.
            ///     Initialisiert eine neue Instanz der <see cref="StartUpHelper{T}" /> Klasse.
            ///     Initializes a new instance of the <see cref="StartUpHelper{T}" /> class.
            /// </summary>
            /// <param name="helper">
            ///     The helper.
            /// </param>
            public StartUpHelper([NotNull] ApplicationHelper helper)
            {
                if (helper == null) throw new ArgumentNullException(nameof(helper));
                _helper = helper;
            }

            #endregion

            #region Public Methods and Operators

            /// <summary>The start.</summary>
            /// <returns>
            ///     The <see cref="T" />.
            /// </returns>
            [NotNull]
            public T Start()
            {
                _helper.RunUIThread(Starthelper);
                return _temp;
            }

            #endregion

            #region Methods

            /// <summary>The starthelper.</summary>
            private void Starthelper()
            {
                _temp = Activator.CreateInstance<T>();
                _helper.RunAnonymousApplication(_temp);
            }

            #endregion

            #region Fields

            /// <summary>The _helper.</summary>
            private readonly ApplicationHelper _helper;

            /// <summary>The _temp.</summary>
            private T _temp;

            #endregion
        }

        #region Fields

        /// <summary>The _factory.</summary>
        [Inject]
        private IUIControllerFactory _factory;

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The create ui thread.
        /// </summary>
        /// <param name="start">
        ///     The start.
        /// </param>
        /// <returns>
        ///     The <see cref="Thread" />.
        /// </returns>
        public Thread CreateUIThread(ThreadStart start)
        {
            var thread = new Thread(start) {Name = "UI Thread"};
            thread.SetApartmentState(ApartmentState.STA);
            return thread;
        }

        /// <summary>The run anonymous application.</summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>
        ///     The <see cref="IWindow" />.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public IWindow RunAnonymousApplication<T>() where T : class, IWindow
        {
            return new StartUpHelper<T>(this).Start();
        }

        /// <summary>
        ///     The run anonymous application.
        /// </summary>
        /// <param name="window">
        ///     The window.
        /// </param>
        public void RunAnonymousApplication(IWindow window)
        {
            var app = _factory.CreateController();
            app.MainWindow   = window;
            app.ShutdownMode = ShutdownMode.OnMainWindowClose;
            app.Run(window);
        }

        /// <summary>
        ///     The run ui thread.
        /// </summary>
        /// <param name="start">
        ///     The start.
        /// </param>
        public void RunUIThread(ThreadStart start)
        {
            CreateUIThread(start).Start();
        }

        #endregion
    }
}