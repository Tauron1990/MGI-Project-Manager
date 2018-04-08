using System;
using Tauron.Application.Implement;

namespace Tauron.Application.ProjectManager.ApplicationServer
{
    public static class Bootstrapper
    {
        private static Core.Application _app;

        public static bool Start(bool console, IpSettings settings)
        { 
            if(_app != null) throw new InvalidOperationException("Application already Started");

            _app = new Core.Application(console, settings);

            string name = "MGI-Project-Manager";

            if (!SingleInstance<Core.Application>.InitializeAsFirstInstance(name, _app)) return false;

            _app.Run();

            return true;

        }

        public static void Stop()
        {
            if(_app == null) return;

            _app.Shutdown();

            _app = null;
        }
    }
}