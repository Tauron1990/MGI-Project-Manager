using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Anotar.Serilog;
using JKang.IpcServiceFramework;
using MessagePack;
using Microsoft.DotNet.PlatformAbstractions;
using Microsoft.Extensions.DependencyInjection;
using Scrutor;
using Tauron.Application.Deployment.AutoUpload.Models.Github;

namespace Tauron.Application.Deployment.AutoUpload.Models.Build
{
    [ServiceDescriptor(typeof(BuildContext), ServiceLifetime.Singleton)]
    public sealed class BuildContext
    {
        private const string DotNetLocation = @"C:\Program Files\dotnet\dotnet.exe";

        public static bool CanBuild => File.Exists(DotNetLocation);

        public event Action<string>? Output;

        public event Action? Error;

        private readonly IServiceScopeFactory _factory;

        public BuildContext(IServiceScopeFactory factory) 
            => _factory = factory;

        public async Task<int> TryBuild(RegistratedRepository? repository, string outputRoot)
        {
            //TODO Switch to Self Start
            LogTo.Information("Begin Build Repository: {Repository}", repository?.ToString());
            using var scope = _factory.CreateScope();
            using var token = new CancellationTokenSource();
            string pipeName = Guid.NewGuid().ToString("B");
            // ReSharper disable once UnusedVariable
            var host = GetHost(scope.ServiceProvider, token, pipeName);
            var dispatcher = scope.ServiceProvider.GetRequiredService<BuildDispatcher>();
            dispatcher.MessageRecived += InfoReciverOnMessageRecivedEvent;

            try
            {
                var targetFile = repository?.ProjectName;
                if (targetFile == null) return -1;

                var result = 0;

                LogTo.Information("Read Build Information");
                await foreach (var (fileName, relativeOutput) in BuildFile.GetEntrysForRepository(Argument.NotNull(repository, nameof(repository))).WithCancellation(token.Token))
                {
                    if (result != 0) continue;

                    var outputPath = Path.Combine(outputRoot, relativeOutput);
                    if (!Directory.Exists(outputPath))
                        Directory.CreateDirectory(outputPath);

                    LogTo.Information("Start build: {File}", fileName);
                    var execResult = await TryBuild(fileName, outputPath, pipeName);
                    if (execResult != 0)
                        Interlocked.Exchange(ref result, execResult);
                }

                return result;
            }
            finally
            {
                dispatcher.MessageRecived -= InfoReciverOnMessageRecivedEvent;
                token.Cancel();
            }
        }

        private static async Task<IIpcServiceHost> GetHost(IServiceProvider serviceProvider, CancellationTokenSource tokenSource, string pipeName)
        {
            LogTo.Information("Create Ipc Host");
            var builder = new IpcServiceHostBuilder(serviceProvider);

            var host = builder.AddNamedPipeEndpoint<IBuildServer>(pipeName, pipeName).Build();
            await host.RunAsync(tokenSource.Token);
            return host;
        }

        private static async Task<int> TryBuild(string? projectFile, string output, string pipeName)
        {
            var info = new BuildInfo(output, pipeName, projectFile ?? string.Empty);
            // ReSharper disable once UseAwaitUsing
            using (var file = File.Open(Path.GetFullPath(BuildInfo.BuildFile, ApplicationEnvironment.ApplicationBasePath), FileMode.Create)) await MessagePackSerializer.SerializeAsync(file, info);

            LogTo.Information("Start Building process");
            using var process = new Process {StartInfo = new ProcessStartInfo(Path.GetFullPath("ProjectBuilder.exe", ApplicationEnvironment.ApplicationBasePath))};
            process.Start();
            await Task.Delay(1000);
            if (process.WaitForExit(30000)) return process.ExitCode;

            LogTo.Warning("Timeout Killing Build process");
            process.Kill(true);

            return process.ExitCode;
        }

        private void  InfoReciverOnMessageRecivedEvent(string arg)
        {
            var msg = arg;
            if (msg == "error")
                Error?.Invoke();
            else
                Output?.Invoke(msg);
        }
    }
}