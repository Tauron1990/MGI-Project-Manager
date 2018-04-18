using System;
using System.Threading;
using System.Threading.Tasks;
using Tauron.Application.ProjectManager.AdminClient;

namespace Multistarter
{
    internal class Program
    {
        [STAThread]
        private static void Main(string[] args)
        {
            var curr   = AppDomain.CurrentDomain;
            var domain = AppDomain.CreateDomain("Server", curr.Evidence.Clone(), curr.SetupInformation);
            Task.Run(() => domain.DoCallBack(ServerStartHelper.Program.Main));

            Thread.Sleep(5000);
            Programm.DEBUGSTART();
        }
    }
}