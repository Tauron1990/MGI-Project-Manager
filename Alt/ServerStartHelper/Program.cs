using System;
using Tauron.Application.ProjectManager;
using Tauron.Application.ProjectManager.ApplicationServer;

namespace ServerStartHelper
{
    public class Program
    {
        public static void Main()
        {
            Console.Title = "Server";
            try
            {
                Console.WindowWidth = 180;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            if (!Bootstrapper.Start(true, IpSettings.CreateDefault())) return;

            Console.ReadKey();
            Bootstrapper.Stop();
        }
    }
}