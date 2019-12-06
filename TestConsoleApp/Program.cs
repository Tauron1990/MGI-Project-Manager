using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Text;

namespace TestConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Process.Start(@"C:\Program Files\dotnet\dotnet.exe", "publish \"C:\\Users\\user\\AppData\\Roaming\\Tauron\\Tauron.Application.Deployment.AutoUpload\\Repos\\AutoFanControl\\master\\Auto Fan Control\\Auto Fan Control.csproj\" -o \"C:\\Users\\user\\Source\\Repos\\MGI-Project-Manager\\Tauron.Application.Deployment.AutoUpload\\bin\\Debug\\netcoreapp3.1\\Output\" -c Release -v n");

            Console.ReadKey();
        }
    }
}
