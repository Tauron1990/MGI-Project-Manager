using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Octokit;
using Tauron.Application.Deployment.AutoUpload.Build;
using Tauron.Application.Deployment.AutoUpload.Core;
using Tauron.Application.Deployment.AutoUpload.Github;

namespace Tauron.Application.Deployment.AutoUpload
{
    public delegate Task<bool> OperationFunction(ApplicationContext context);

    public static class Program
    {
        private class ReportHelper
        {
            private readonly StringBuilder _stringBuilder = new StringBuilder();
            private readonly ConsoleUi _ui;
            
            private readonly List<string> _output = new List<string>();
            private string _line = string.Empty;

            public ReportHelper(ConsoleUi ui) 
                => _ui = ui;

            public void Line(string line)
            {
                _line = line;
                Update();
            }

            public void AddOutput(string output)
            {
                _output.Add(output);
                Update();
            }

            private void Update()
            {
                _stringBuilder.Clear()
                    .AppendLine();

                foreach (var line in _output) 
                    _stringBuilder.AppendLine(line);

                _stringBuilder.AppendLine(_line);

                _ui.ReplaceLast(_stringBuilder.ToString());
            }
        }

        private static readonly GitHubClient GitHubClient = new GitHubClient(new ProductHeaderValue("TaTauron.Application.Deployment.AutoUpload"));
        private static readonly ConsoleUi UI = new ConsoleUi();

        public static async Task Main()
        {
            UI.Title = "Auto Versionen und Uploads";

            var context = ApplicationContext.Create();

            if (context != null)
            {

                var run = true;

                while (run)
                {
                    var command = UI.WriteLine(3).ReadLine("Kommando: ");

                    var operation = command switch
                    {
                        "help" => new OperationFunction(HelpCommand),
                        "add" => new OperationFunction(AddCommand),
                        "exit" => new OperationFunction(ExitCommand),
                        _ => new OperationFunction(c => Task.FromResult(true))
                    };

                    try
                    {
                        run = await operation(context);
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
                if (UI.Allow(website + " Öffnen:"))
                    OpenWebsite(website);
            }
        }

        //private static Task<Result<bool, Exception>> AddCommand(ApplicationContext context)
        //{

        //}

        private static Task<bool> HelpCommand(ApplicationContext context)
        {
            UI
                .Clear()
                .WriteLine(3)
                .WriteLine("Hilfe Commandos:")
                .Write("exit => ").WriteLine("Beendet da Programm")
                .Write("add => ").WriteLine("Fügt ein neues Projeckt hinzu");

            return Task.FromResult(true);
        }

        private static async Task<bool> AddCommand(ApplicationContext context)
        {
            var name = UI
                .Clear()
                .WriteLine(3)
                .PrintList(context.Settings.KnowenRepositorys, (s, ui) => ui.WriteLine(s))
                .ReadLine("Bitte ein Repository Angeben ({UserName}/{RepoName}): ");

            if (string.IsNullOrWhiteSpace(name)) throw new CommonError("Der Name war Leer");

            var repository = await RepositoryManager.GetRepository(name, GitHubClient);
            await context.Settings.AddRepoAndSave(repository.FullName);
            var branch = await RepositoryManager.GetBranch(repository, () => UI.ReadLine("Name des Branches: "), GitHubClient);

            string tempPath = Path.Combine(Path.GetTempPath(), "Tauron");

            try
            {
                var repothelper = new ReportHelper(UI);
                UI.WriteLine();
                var target = GitManager.SyncBranch(repository.CloneUrl, branch.Name, tempPath,
                    output =>
                    {
                         UI.WriteLine(output);
                         return true;
                    }, progress =>
                    {
                        UI.WriteLine($"Objects: {progress.ReceivedObjects}/{progress.TotalObjects} -- Bytes: {progress.ReceivedBytes}");
                        return true;
                    });
            }
            finally
            {
                Directory.Delete(tempPath, true);
            }

            return true;
        }

        private static Task<bool> ExitCommand(ApplicationContext context) => Task.FromResult(false);

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
