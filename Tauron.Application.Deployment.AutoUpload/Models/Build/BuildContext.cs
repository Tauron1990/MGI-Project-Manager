using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Scrutor;
using Tauron.Application.Deployment.AutoUpload.Models.Github;

namespace Tauron.Application.Deployment.AutoUpload.Models.Build
{
    [ServiceDescriptor(typeof(BuildContext), ServiceLifetime.Singleton)]
    public sealed class BuildContext
    {
        private const string DotNetLocation = @"C:\Program Files\dotnet\dotnet.exe";

        public event Action<string> Output;

        public event Action Error;

        public static bool CanBuild => File.Exists(DotNetLocation);

        public async Task<int> TryBuild(RegistratedRepository? repository, string output)
        {
            var arguments = new StringBuilder()
               .Append(DotNetLocation)
               .Append("publish ")
               .Append(repository?.ProjectName)
               .Append($" -o {output}")
               .Append(" -c Release")
               .Append(" -v n");

            using var process = new Process();

            process.ErrorDataReceived += ProcessOnErrorDataReceived;
            process.OutputDataReceived += ProcessOnOutputDataReceived;
            process.EnableRaisingEvents = true;


            process.StartInfo = new ProcessStartInfo(DotNetLocation, arguments.ToString())
                                {
                                    UseShellExecute = false,
                                    RedirectStandardError = true,
                                    RedirectStandardOutput = true
                                };

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            if (!process.WaitForExit(30000)) 
                process.Kill(true);

            return process.ExitCode;
        }

        private void ProcessOnOutputDataReceived(object sender, DataReceivedEventArgs e) 
             => Output?.Invoke(e.Data);

        private void ProcessOnErrorDataReceived(object sender, DataReceivedEventArgs e) => Error?.Invoke();
    }
}