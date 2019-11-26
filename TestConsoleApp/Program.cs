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
        private const string DotNetLocation = @"C:\Program Files\dotnet\dotnet.exe";
        private static int errorCount;

        static void Main(string[] args)
        {
            using (var process = new Process())
            {
                process.ErrorDataReceived += ProcessOnErrorDataReceived;
                process.OutputDataReceived += ProcessOnOutputDataReceived;
                process.EnableRaisingEvents = true;

                var arguments = new StringBuilder()
                    .Append("publish ")
                    .Append(@"C:\Users\PC\Desktop\test\MGI\Tauron.Application.Deployment.AutoUpload\Tauron.Application.Deployment.AutoUpload.csproj").Append(" ")
                    .Append($"-o " + @"C:\test")
                    .Append(" -c Release")
                    .Append(" -v n");

                process.StartInfo = new ProcessStartInfo(DotNetLocation, arguments.ToString())
                {
                    UseShellExecute = false,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true
                };

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                if (!process.WaitForExit(10000))
                {
                    process.Kill(true);
                }


                Console.WriteLine(process.ExitCode);
                Console.WriteLine(errorCount);
            }

            Console.ReadKey();
        }

        private static void ProcessOnOutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            Console.WriteLine(e.Data);
        }

        private static void ProcessOnErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            errorCount++;
            Console.WriteLine(e.Data);
        }
    }
}
