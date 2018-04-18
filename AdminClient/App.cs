using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using JetBrains.Annotations;
using NLog;
using NLog.Config;
using NLog.Targets;
using Tauron.Application.Implement;
using Tauron.Application.Implementation;
using Tauron.Application.Ioc;
using Tauron.Application.ProjectManager.Generic;
using Tauron.Application.ProjectManager.Generic.Windows;
using Tauron.Application.ProjectManager.Resources;
using Tauron.Application.Views;

namespace Tauron.Application.ProjectManager.AdminClient
{
    internal class App : WpfApplication, ISingleInstanceApp
    {
        public App()
            : base(true)
        {
        }

        public bool SignalExternalCommandLineArgs(IList<string> args)
        {
            MainWindow?.Focus();
            return true;
        }

        public static void Setup([NotNull] Mutex mutex, [NotNull] string channelName)
        {
            if (mutex == null) throw new ArgumentNullException(nameof(mutex));
            if (channelName == null) throw new ArgumentNullException(nameof(channelName));

            Run<App>(app => SingleInstance<App>.InitializeAsFirstInstance(mutex, channelName, app), CultureInfo.GetCultureInfo("de-de"));
        }

        protected override void ConfigSplash()
        {
            var dic = new PackUriHelper().Load<ResourceDictionary>("StartResources.xaml");

            CurrentWpfApplication.Resources = dic;

            var control = new ContentControl
                          {
                              HorizontalContentAlignment = HorizontalAlignment.Center,
                              VerticalContentAlignment   = VerticalAlignment.Center,
                              Height                     = 236,
                              Width                      = 414,
                              Content                    = dic["MainLabel"]
                          };

            SplashMessageListener.CurrentListner.SplashContent       = control;
            SplashMessageListener.CurrentListner.MainLabelForeground = "Black";
            SplashMessageListener.CurrentListner.MainLabelBackground = dic["MainLabelbackground"];
        }

        protected override IWindow DoStartup(CommandLineProcessor prcessor)
        {
            var temp = ViewManager.Manager.CreateWindow(AppConststands.MainWindowName);
            MainWindow = temp;

            CurrentWpfApplication.Dispatcher.Invoke(() =>
                                                    {
                                                        Current.MainWindow               = temp;
                                                        CurrentWpfApplication.MainWindow = (Window) temp.TranslateForTechnology();
                                                    });
            return temp;
        }

        protected override void LoadCommands()
        {
            base.LoadCommands();
            CommandBinder.AutoRegister = true;
        }

        protected override void LoadResources()
        {
            SimpleLocalize.Register(AdminClientLabels.ResourceManager, typeof(App).Assembly);
            SimpleLocalize.Register(UIMessages.ResourceManager, typeof(LogInWindow).Assembly);

            System.Windows.Application.Current.Resources.MergedDictionaries.Add(
                                                                                (ResourceDictionary)
                                                                                System.Windows.Application.LoadComponent(new PackUriHelper().GetUri("Theme.xaml", typeof(App).Assembly.FullName,
                                                                                                                                                    false)));
        }

        public override string GetdefaultFileLocation()
        {
            return GetDicPath();
        }

        protected override void MainWindowClosed(object sender, EventArgs e)
        {
            base.MainWindowClosed(sender, e);

            OnExit();
        }

        private static string GetDicPath()
        {
            return
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)
                           .CombinePath("Tauron\\MGIProjectManager");
        }

        protected override void Fill(IContainer container)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));

            var resolver = new ExportResolver();

            resolver.AddAssembly(typeof(App).Assembly);
            resolver.AddAssembly(typeof(WpfApplication).Assembly);
            resolver.AddAssembly(typeof(CommonApplication).Assembly);
            resolver.AddAssembly(typeof(DialogFactory).Assembly);
            resolver.AddAssembly(typeof(ClientFactory).Assembly);

            container.Register(resolver);
        }

        protected override void ConfigurateLagging(LoggingConfiguration config)
        {
            var filetarget = new FileTarget
                             {
                                 Name             = "CommonFile",
                                 Layout           = "${log4jxmlevent}",
                                 ArchiveAboveSize = 10485760,
                                 MaxArchiveFiles  = 10,
                                 ArchiveFileName  = GetdefaultFileLocation().CombinePath("Logs\\AdminClient.{##}.log"),
                                 FileName         = GetdefaultFileLocation().CombinePath("Logs\\AdminClient.log"),
                                 ArchiveNumbering = ArchiveNumberingMode.Rolling
                             };


            config.AddTarget(filetarget);
            config.LoggingRules.Add(new LoggingRule("*", LogLevel.Trace, filetarget));

            #if DEBUG
            LogManager.ThrowExceptions = true;
            LogManager.GlobalThreshold = LogLevel.Trace;
            #endif
        }
    }
}