﻿using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Serilog;
using Tauron.Application.Logging;

namespace Tauron.Application.ToolUI
{
    public static class CoreProgram
    {
        public static async Task Main(string[] args)
        {
            using var logger = new LoggerConfiguration().ConfigDefaultLogging("ToolUI").CreateLogger();
            Log.Logger = logger;

            var host = Host.CreateDefaultBuilder(args)
               .UseWpf<MainWindow>(configuration => configuration.WithAppFactory(() => new App()))
               .UseStartUp<Startup>()
               .ConfigureLogging(b => b.AddSerilog())
               .Build();
            await host.RunAsync();
        }
    }
}