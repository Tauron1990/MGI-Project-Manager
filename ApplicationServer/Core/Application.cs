using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Forms;
using NLog;
using NLog.Config;
using NLog.Targets;
using Tauron.Application.Common.BaseLayer.BusinessLayer;
using Tauron.Application.Implement;
using Tauron.Application.Ioc;
using Tauron.Application.ProjectManager.ApplicationServer.Data;

namespace Tauron.Application.ProjectManager.ApplicationServer.Core
{
    internal sealed class Application : CommonApplication, ISingleInstanceApp
    {
        private readonly bool             _enableConsole;
        private readonly ServiceContainer _serviceContainer = new ServiceContainer();
        private readonly IpSettings       _settings;

        /// <inheritdoc />
        public Application(bool enableConsole, IpSettings settings) : base(true, null, new UIControllerFactoryFake())
        {
            _enableConsole = enableConsole;
            _settings      = settings;
        }

        public override IContainer Container { get; set; }

        public bool SignalExternalCommandLineArgs(IList<string> args) => true;

        public override string GetdefaultFileLocation() => GetDicPath();

        private static string GetDicPath() => Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)
                                                         .CombinePath("Tauron\\MGIProjectManager");

        protected override void Fill(IContainer container)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));

            var resolver = new ExportResolver();

            resolver.AddAssembly(typeof(Application).Assembly);
            resolver.AddAssembly(typeof(CommonApplication).Assembly);
            resolver.AddAssembly(typeof(RuleFactory).Assembly);
            resolver.AddAssembly(typeof(ServiceContainer).Assembly);

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
                                 ArchiveFileName  = GetdefaultFileLocation().CombinePath("Logs\\ApplicationServer.{##}.log"),
                                 FileName         = GetdefaultFileLocation().CombinePath("Logs\\ApplicationServer.log"),
                                 ArchiveNumbering = ArchiveNumberingMode.Rolling
                             };


            config.AddTarget(filetarget);
            config.LoggingRules.Add(new LoggingRule("*", LogLevel.Trace, filetarget));

            Bootstrapper.OnConfigurateLogging(config);

            if (!_enableConsole) return;

            var consoleTarget = new ConsoleTarget {DetectConsoleAvailable = true, Name = "Console"};
            config.AddTarget(consoleTarget);
            config.LoggingRules.Add(new LoggingRule("*", LogLevel.Trace, consoleTarget));
        }

        protected override void OnStartupError(Exception e)
        {
            MessageBox.Show($"{e.GetType()} : {e.Message}", e.GetType().Name);
        }

        protected override IWindow DoStartup(CommandLineProcessor args)
        {
            DatabaseImpl.UpdateSchema();

            _serviceContainer.Start(_settings);

            LogManager.GetCurrentClassLogger().Info($"Start Compled - Lang: {CultureInfo.CurrentCulture}");

            return null;
        }

        public override void Shutdown()
        {
            _serviceContainer.Stop();
            OnExit();

            base.Shutdown();
        }

        public void Run()
        {
            OnStartup(new string[0]);
        }
    }
}