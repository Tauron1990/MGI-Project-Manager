using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Threading.Tasks;
using Catel;
using MessagePack;
using Microsoft.DotNet.PlatformAbstractions;
using Microsoft.Extensions.DependencyInjection;
using Scrutor;
using Tauron.Application.Deployment.AutoUpload.Models.Github;
using Tauron.Application.Pipes;
using Tauron.Application.Pipes.IO;

namespace Tauron.Application.Deployment.AutoUpload.Models.Build
{
    [ServiceDescriptor(typeof(BuildContext), ServiceLifetime.Singleton)]
    public sealed class BuildContext
    {
        private const string DotNetLocation = @"C:\Program Files\dotnet\dotnet.exe";

        public event Action<string>? Output;

        public event Action? Error;

        public static bool CanBuild => File.Exists(DotNetLocation);

        public async Task<int> TryBuild(RegistratedRepository? repository, string output)
        {
            using var infoReciver = new PipeServer<string>(Anonymos.Create(PipeDirection.In, out var pipeName));
            infoReciver.MessageRecivedEvent += InfoReciverOnMessageRecivedEvent;
            await infoReciver.Connect();

            var info = new BuildInfo(output, pipeName, repository?.ProjectName ?? string.Empty);
            // ReSharper disable once UseAwaitUsing
            using (var file = File.Open(Path.GetFullPath(BuildInfo.BuildFile, ApplicationEnvironment.ApplicationBasePath), FileMode.Create)) 
                await MessagePackSerializer.SerializeAsync(file, info);

            using var process = new Process {StartInfo = new ProcessStartInfo(Path.GetFullPath("ProjectBuilder.exe", ApplicationEnvironment.ApplicationBasePath))};
            process.Start();
            await Task.Delay(1000);
            if (!process.WaitForExit(30000)) 
                process.Kill(true);

            return process.ExitCode;
        }

        private Task InfoReciverOnMessageRecivedEvent(MessageRecivedEventArgs<string> arg)
        {
            var msg = arg.Message;
            if(msg == "error")
                Error?.Invoke();
            else
                Output?.Invoke(msg);

            return Task.CompletedTask;
        }
    }
}