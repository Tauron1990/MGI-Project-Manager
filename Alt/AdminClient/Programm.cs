using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.ExceptionServices;
using System.Security;
using System.Security.Principal;
using System.Threading;
using JetBrains.Annotations;
using Tauron.Application.Implement;

namespace Tauron.Application.ProjectManager.AdminClient
{
    public static class Programm
    {
        [STAThread]
        [LoaderOptimization(LoaderOptimization.SingleDomain)]
        public static void Main()
        {
            var applicationIdentifier = "PrejectManager.AdminClient";
            if (SecurityHelper.IsEnvironmentPermissionGranted())
                applicationIdentifier += Environment.UserName;

            try
            {
                var channelName = string.Concat(applicationIdentifier, ":", "SingeInstanceIPCChannel");
                var mutex       = new Mutex(true, applicationIdentifier, out var first);

                try
                {
                    if (!first)
                        SignalFirstInstance(channelName, applicationIdentifier);

                    var domain = AppDomain.CurrentDomain;
                    domain.SetPrincipalPolicy(PrincipalPolicy.WindowsPrincipal);
                    domain.UnhandledException += OnUnhandledException;

                    StartApp(mutex, channelName);
                }
                finally
                {
                    CleanUp();
                }
            }
            catch (MethodAccessException)
            {
                CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("de-de");
                WpfApplication.Run<App>();
            }
        }

        [Conditional("DEBUG")]
        public static void DEBUGSTART()
        {
            WpfApplication.Run<App>(null, new CultureInfo("de-de"));
        }

        private static void SignalFirstInstance([NotNull] string channelName, [NotNull] string applicationIdentifier)
        {
            if (channelName == null) throw new ArgumentNullException(nameof(channelName));
            if (applicationIdentifier == null) throw new ArgumentNullException(nameof(applicationIdentifier));

            SingleInstance<App>.SignalFirstInstance(channelName,
                                                    SingleInstance<App>.GetCommandLineArgs(applicationIdentifier));
        }

        private static void CleanUp()
        {
            SingleInstance<App>.Cleanup();
        }


        private static void StartApp([NotNull] Mutex mutex, [NotNull] string channelName)
        {
            App.Setup(mutex, channelName);
        }

        [HandleProcessCorruptedStateExceptions]
        [SecurityCritical]
        private static void OnUnhandledException([NotNull] object sender, [NotNull] UnhandledExceptionEventArgs args)
        {
            CommonConstants.LogCommon(true, args.ExceptionObject.ToString());
        }
    }
}