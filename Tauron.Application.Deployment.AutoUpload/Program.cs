using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Octokit;
using Tauron.Application.Deployment.AutoUpload.Build;
using Tauron.Application.Deployment.AutoUpload.Commands;

namespace Tauron.Application.Deployment.AutoUpload
{
    public delegate Task<bool> OperationFunction(ApplicationContext context);

    public static class Program
    {
        private static readonly ConsoleUi UI = new ConsoleUi();

        public static async Task Main()
        {
            UI.Title = "Auto Versionen und Uploads";

            var context = ApplicationContext.Create();
            if (context != null)
            {
                var commadManager = new CommandManager(UI, context);
                var run = true;

                while (run)
                {
                    var command = await UI.WriteLine(3).ReadLine("Kommando: ");

                    if (command == "help")
                    {
                        commadManager.PrintHelp();
                        continue;
                    }

                    try
                    {
                        run = await commadManager.ExecuteNext(command);
                    }
                    catch (Exception e)
                    {
                        UI.WriteError(e);
                    }
                }
            }
            else
            {

                const string website = "https://dotnet.microsoft.com/download";
                UI.WriteLine("Dot Net Core Framework nicht gefunden.");
                if (await UI.Allow(website + " Öffnen:"))
                    OpenWebsite(website);
            }
        }

        private static void OpenWebsite(string url)
        {
            try
            {
                Process.Start(url);
            }
            catch
            {
                // hack because of this: https://github.com/dotnet/corefx/issues/10361
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    url = url.Replace("&", "^&");
                    Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Process.Start("xdg-open", url);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Process.Start("open", url);
                }
                else
                {
                    throw;
                }
            }
        }
    }
}
