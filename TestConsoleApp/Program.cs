using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management.Automation;

namespace TestConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = "powershell";
            psi.UseShellExecute = false;
            psi.RedirectStandardOutput = true;

            psi.Arguments = Path.GetFullPath("dotnet-install.ps1 -Help");
            Process p = Process.Start(psi);
            string strOutput = p.StandardOutput.ReadToEnd();
            p.WaitForExit();
            Console.WriteLine(strOutput);


            Console.ReadKey();
        }
    }
}
