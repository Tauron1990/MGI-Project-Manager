// The file WpfApplication.cs is part of Tauron.Application.Common.Wpf.
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
using System.Globalization;
using System.Threading;
using System.Windows;
using JetBrains.Annotations;
using NLog;
using NLog.Config;
using NLog.Targets;
using Tauron.Application.Composition;
using Tauron.Application.Implementation;
using Tauron.Application.Ioc;

#endregion

namespace Tauron.Application
{
    /// <summary>The wpf application.</summary>
    [PublicAPI]
    public class WpfApplication : CommonApplication
    {
        #region Public Methods and Operators

        /// <summary>The run.</summary>
        public static void Run<TApp>(Action<TApp> runBeforStart = null, CultureInfo info = null) where TApp : WpfApplication, new()
        {
            WpfApplicationController.Initialize(info);

            if (info != null && !info.Equals(CultureInfo.InvariantCulture))
            {
                Thread.CurrentThread.CurrentCulture   = info;
                Thread.CurrentThread.CurrentUICulture = info;
            }

            var app = new TApp();
            runBeforStart?.Invoke(app);
            UiSynchronize.Synchronize.Invoke(app.ConfigSplash);
            app.OnStartup(Environment.GetCommandLineArgs());
        }

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="WpfApplication" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="WpfApplication" /> Klasse.
        /// </summary>
        /// <param name="doStartup">
        ///     The do startup.
        /// </param>
        public WpfApplication(bool doStartup)
            : base(doStartup, new SplashService(), new WpfIuiControllerFactory())
        {
            CatalogList = "Catalogs.xaml";
        }

        public WpfApplication(bool doStartup, System.Windows.Application app)
            : base(doStartup, new SplashService(), new WpfIuiControllerFactory(app))
        {
            CatalogList = "Catalogs.xaml";
        }

        #endregion

        #region Public Properties

        /// <summary>Gets or sets the container.</summary>
        public override IContainer Container
        {
            get => CompositionServices.Container;

            set => CompositionServices.Container = value;
        }

        /// <summary>Gets or sets the theme dictionary.</summary>
        [CanBeNull]
        public string ThemeDictionary { get; set; }

        [NotNull]
        public static System.Windows.Application CurrentWpfApplication => System.Windows.Application.Current;

        #endregion

        #region Methods

        /// <summary>The config splash.</summary>
        protected virtual void ConfigSplash()
        {
        }

        /// <summary>The load resources.</summary>
        protected override void LoadResources()
        {
            if (string.IsNullOrEmpty(ThemeDictionary)) return;

            QueueWorkitemAsync(
                               () =>
                                   WpfApplicationController.Application.Resources.MergedDictionaries.Add(
                                                                                                         System.Windows.Application
                                                                                                               .LoadComponent(
                                                                                                                              new Uri
                                                                                                                                  (
                                                                                                                                   $@"/{SourceAssembly};component/{ThemeDictionary}",
                                                                                                                                   UriKind
                                                                                                                                       .Relative))
                                                                                                               .CastObj
                                                                                                                   <ResourceDictionary>
                                                                                                                   ()),
                               true);
        }


        protected override void MainWindowClosed(object sender, EventArgs e)
        {
            Shutdown();
        }

        public override void Shutdown()
        {
            base.Shutdown();
            System.Windows.Application.Current.Shutdown();
            Container.Dispose();
        }

        protected override void ConfigurateLagging(LoggingConfiguration config)
        {
            var filetarget = new FileTarget
                             {
                                 Name             = "CommonFile",
                                 Layout           = "${log4jxmlevent}",
                                 ArchiveAboveSize = 10485760,
                                 MaxArchiveFiles  = 10,
                                 ArchiveFileName  = GetdefaultFileLocation().CombinePath("Logs\\Tauron.Application.Common.{##}.log"),
                                 FileName         = GetdefaultFileLocation().CombinePath("Logs\\Tauron.Application.Common.log"),
                                 ArchiveNumbering = ArchiveNumberingMode.Rolling
                             };
            config.AddTarget(filetarget);

            config.LoggingRules.Add(new LoggingRule("*", LogLevel.Trace, filetarget));
        }

        protected override IContainer CreateContainer()
        {
            var con = base.CreateContainer();

            con.Register(new PropertyModelExtension());

            return con;
        }

        protected override void OnStartupError(Exception e)
        {
            MessageBox.Show(e.ToString());
        }

        #endregion
    }
}