using System;
using Tauron.Application.ProjectManager;
using Tauron.Application.ProjectManager.ApplicationServer;

namespace ServerStartHelper
{
    public class Program
    {
        public static void Main()
        {
            if (!Bootstrapper.Start(true, IpSettings.CreateDefault())) return;

            Console.ReadKey();
            Bootstrapper.Stop();
        }
    }
}