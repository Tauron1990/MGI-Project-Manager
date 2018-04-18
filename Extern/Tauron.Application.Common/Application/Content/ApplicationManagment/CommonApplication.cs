using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Xaml;
using JetBrains.Annotations;
using NLog;
using NLog.Config;
using Tauron.Application.Composition;
using Tauron.Application.Implement;
using Tauron.Application.Ioc;
using Tauron.Application.Modules;

namespace Tauron.Application
{
    /// <summary>The common application.</summary>
    [PublicAPI]
    public abstract class CommonApplication
    {
        /// <summary>The null splash.</summary>
        private class NullSplash : ISplashService
        {
            #region Constructors and Destructors

            /// <summary>
            ///     Initializes a new instance of the <see cref="NullSplash" /> class.
            ///     Initialisiert eine neue Instanz der <see cref="NullSplash" /> Klasse.
            ///     Initializes a new instance of the <see cref="NullSplash" /> class.
            /// </summary>
            public NullSplash()
            {
                Listner = new SplashMessageListener();
            }

            #endregion

            #region Public Properties

            /// <summary>Gets the listner.</summary>
            /// <value>The listner.</value>
            public SplashMessageListener Listner { get; private set; }

            #endregion

            #region Public Methods and Operators

            /// <summary>The close splash.</summary>
            public void CloseSplash()
            {
            }

            /// <summary>The show splash.</summary>
            public void ShowSplash()
            {
            }

            #endregion
        }

        #region Static Fields

        /// <summary>The _scheduler.</summary>
        private static TaskScheduler _scheduler;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initialisiert eine neue Instanz der <see cref="CommonApplication" /> Klasse.
        /// </summary>
        /// <param name="doStartup">
        ///     The do startup.
        /// </param>
        /// <param name="service">
        ///     The service.
        /// </param>
        /// <param name="factory">
        ///     The factory.
        /// </param>
        protected CommonApplication(bool doStartup, [CanBeNull] ISplashService service, [NotNull] IUIControllerFactory factory)
        {
            Factory        = factory ?? throw new ArgumentNullException(nameof(factory));
            Current        = this;
            _scheduler     = new TaskScheduler(UiSynchronize.Synchronize);
            _splash        = service ?? new NullSplash();
            _doStartup     = doStartup;
            SourceAssembly = new AssemblyName(Assembly.GetAssembly(GetType()).FullName).Name;
        }

        #endregion

        #region Properties

        /// <summary>Gets or sets the source assembly.</summary>
        /// <value>The source assembly.</value>
        [NotNull]
        protected static string SourceAssembly { get; set; }

        #endregion

        public virtual string GetdefaultFileLocation()
        {
            return AppDomain.CurrentDomain.BaseDirectory;
        }

        #region Fields

        /// <summary>The _do startup.</summary>
        private readonly bool _doStartup;

        /// <summary>The _splash.</summary>
        private readonly ISplashService _splash;

        /// <summary>The _args.</summary>
        private string[] _args;

        #endregion

        #region Public Properties

        /// <summary>Gets the current.</summary>
        /// <value>The current.</value>
        [NotNull]
        public static CommonApplication Current { get; private set; }

        /// <summary>Gets the scheduler.</summary>
        /// <value>The scheduler.</value>
        [NotNull]
        public static TaskScheduler Scheduler => _scheduler ?? (_scheduler = new TaskScheduler());

        /// <summary>Gets or sets the catalog list.</summary>
        /// <value>The catalog list.</value>
        [CanBeNull]
        public string CatalogList { get; set; }

        /// <summary>Gets or sets the container.</summary>
        /// <value>The container.</value>
        [NotNull]
        public abstract IContainer Container { get; set; }

        /// <summary>Gets or sets the factory.</summary>
        [NotNull]
        public IUIControllerFactory Factory { get; private set; }

        [CanBeNull]
        public IWindow MainWindow { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The queue workitem.
        /// </summary>
        /// <param name="action">
        ///     The action.
        /// </param>
        /// <param name="withDispatcher">
        ///     The with dispatcher.
        /// </param>
        /// <returns>
        ///     The <see cref="Task" />.
        /// </returns>
        [NotNull]
        public static Task QueueWorkitemAsync([NotNull] Action action, bool withDispatcher)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            return Scheduler.QueueTask(new UserTask(action, withDispatcher));
        }

        public static Task QueueWorkitemAsync<TResult>([NotNull] Func<TResult> action, bool withDispatcher)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            return Scheduler.QueueTask(new UserResultTask<TResult>(action, withDispatcher));
        }

        /// <summary>The get args.</summary>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        [NotNull]
        public string[] GetArgs()
        {
            return (string[]) _args.Clone();
        }

        #endregion

        #region Methods

        protected virtual void ConfigurateLagging(LoggingConfiguration config)
        {
        }

        /// <summary>The create container.</summary>
        /// <returns>
        ///     The <see cref="IContainer" />.
        /// </returns>
        [NotNull]
        protected virtual IContainer CreateContainer()
        {
            return new DefaultContainer();
        }

        /// <summary>
        ///     The do startup.
        /// </summary>
        /// <param name="args">
        ///     The args.
        /// </param>
        /// <returns>
        ///     The <see cref="IWindow" />.
        /// </returns>
        [CanBeNull]
        protected virtual IWindow DoStartup([NotNull] CommandLineProcessor args)
        {
            return null;
        }

        /// <summary>
        ///     The fill.
        /// </summary>
        /// <param name="container">
        ///     The container.
        /// </param>
        protected virtual void Fill([NotNull] IContainer container)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));
            if (string.IsNullOrWhiteSpace(CatalogList))
                CommonConstants.LogCommon(false, "Common Application: CatalogList Empty");

            if (CatalogList == null) return;

            var coll = XamlServices.Load(CatalogList) as CatalogCollection;
            try
            {
                if (coll == null) return;

                var resolver = new ExportResolver();
                coll.FillCatalag(resolver);
                container.Register(resolver);
            }
            catch (ArgumentException e)
            {
                LogManager.GetLogger("CommonApplication", typeof(CommonApplication)).Error(e);
                throw;
            }
        }

        /// <summary>The load commands.</summary>
        protected virtual void LoadCommands()
        {
        }

        /// <summary>The load resources.</summary>
        protected virtual void LoadResources()
        {
        }

        /// <summary>
        ///     The main window closed.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="e">
        ///     The e.
        /// </param>
        protected virtual void MainWindowClosed([NotNull] object sender, [NotNull] EventArgs e)
        {
        }

        /// <summary>The on exit.</summary>
        protected virtual void OnExit()
        {
            Container.Dispose();
        }

        /// <summary>
        ///     The on startup.
        /// </summary>
        /// <param name="args">
        ///     The args.
        /// </param>
        protected virtual void OnStartup([NotNull] string[] args)
        {
            if (_doStartup)
            {
                _args = args;
                _splash.ShowSplash();
                _scheduler.QueueTask(new UserTask(PerformStartup, false));
            }

            Scheduler.EnterLoop();
        }

        /// <summary>The shutdown.</summary>
        public virtual void Shutdown()
        {
            Scheduler.Dispose();
        }

        protected virtual void InitializeModule([NotNull] IModule module)
        {
            module.Initialize(this);
            ModuleHandlerRegistry.Progress(module);
        }

        /// <summary>The perform startup.</summary>
        private void PerformStartup()
        {
            try
            {
                var listner = _splash.Listner;

                listner.ReceiveMessage(Resources.Resources.Init_Msg_Step1);

                var config = new LoggingConfiguration();
                ConfigurateLagging(config);
                LogManager.Configuration = config;

                Container = CreateContainer();
                Fill(Container);

                listner.ReceiveMessage(Resources.Resources.Init_Msg_Step2);
                foreach (var module in Container.ResolveAll(typeof(IModule), null)
                                                .Cast<IModule>()
                                                .OrderBy(m => m.Order))
                {
                    InitializeModule(module);
                }

                listner.ReceiveMessage(Resources.Resources.Init_Msg_Step3);
                LoadResources();
                LoadCommands();

                listner.ReceiveMessage(Resources.Resources.Init_Msg_Step4);
                var win = DoStartup(new CommandLineProcessor(this));

                MainWindow = win;

                if (win != null)
                    UiSynchronize.Synchronize.Invoke(
                                                     () =>
                                                     {
                                                         win.Show();
                                                         win.Closed += MainWindowClosed;
                                                     });

                _splash.CloseSplash();
                _args = null;
            }
            catch (Exception e)
            {
                LogManager.GetLogger("CommonApplication", typeof(CommonApplication)).Error(e);

                SplashMessageListener.CurrentListner.Message = e.Message;
                OnStartupError(e);
                Thread.Sleep(2000);
                Scheduler.Dispose();
                Factory.CreateController().Shutdown();
                Environment.Exit(-1);
            }
        }

        protected virtual void OnStartupError([NotNull] Exception e)
        {
        }

        #endregion
    }
}