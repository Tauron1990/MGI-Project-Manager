using System;
using System.Threading.Tasks;
using NLog.Config;
using Tauron.Application.Implement;

namespace Tauron.Application.ProjectManager.ApplicationServer
{
    public static class Bootstrapper
    {
        public static event Action<LoggingConfiguration> ConfigurateLogging; 

        private static Core.Application _app;

        public static bool Start(bool console, IpSettings settings)
        {
            if (_app != null) throw new InvalidOperationException("Application already Started");

            _app = new Core.Application(console, settings);

            var name = "MGI-Project-Manager";

            if (!SingleInstance<Core.Application>.InitializeAsFirstInstance(name, _app)) return false;

            Task.Run(() => _app.Run());

            return true;
        }

        public static void Stop()
        {
            if (_app == null) return;

            _app.Shutdown();

            _app = null;
        }

        internal static void OnConfigurateLogging(LoggingConfiguration obj) => ConfigurateLogging?.Invoke(obj);
    }
}