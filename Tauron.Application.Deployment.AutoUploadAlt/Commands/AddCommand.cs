using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Octokit;
using Tauron.Application.Deployment.AutoUpload.Build;
using Tauron.Application.Deployment.AutoUpload.Core;
using Tauron.Application.Deployment.AutoUpload.Github;

namespace Tauron.Application.Deployment.AutoUpload.Commands
{
    [Command("add", "Fügt ein neuses Projekt zu System hinzu.")]
    public sealed class AddCommand : CommandBase
    {
        private class ReportHelper
        {
            private readonly StringBuilder _stringBuilder = new StringBuilder();
            private readonly ConsoleUi _ui;

            private readonly List<string> _output = new List<string>();
            private string _line = string.Empty;

            private int _updateLimit;

            public ReportHelper(ConsoleUi ui)
                => _ui = ui;

            public void Line(string line)
            {
                _line = line;
                Update();
            }

            public void AddOutput(string output)
            {
                if(_output.Count > 10)
                    _output.RemoveAt(0);
                _output.Add(output);
                Update();
            }

            private void Update()
            {
                _updateLimit++;
                if (_updateLimit != 30) return;

                _updateLimit = 0;

                _stringBuilder.Clear()
                   .AppendLine();

                foreach (var line in _output)
                    _stringBuilder.AppendLine(line);

                _stringBuilder.AppendLine(_line);

                _ui.ReplaceLast(_stringBuilder.ToString());
            }
        }


        public override async Task<bool> Execute(ApplicationContext context, ConsoleUi ui, InputManager input)
        {
            ui
                .Clear()
                .WriteLine(3)
                .PrintList(context.Settings.KnowenRepositorys, (s, ui1) => ui1.WriteLine(s));
            var name = await input.ReadLine("Bitte ein Repository Angeben ({UserName}/{RepoName}): ");

            if (string.IsNullOrWhiteSpace(name)) throw new CommonError("Der Name war Leer");

            var repository = await RepositoryManager.GetRepository(name, context.GitHubClient);
            await context.Settings.AddRepoAndSave(repository.FullName);

            using (ui.SupressUpdate())
            {
                ui.WriteLine().WriteLine("Branches:");

                foreach (var possibleBranch in await context.GitHubClient.Repository.Branch.GetAll(repository.Id)) 
                    ui.WriteLine(possibleBranch.Name);
            }

            var branch = await RepositoryManager.GetBranch(repository, async () => await input.ReadLine("Name des Branches: "), context.GitHubClient);

            var tempPath = Path.Combine(Settings.SettingsDic, "TempGit");

            try
            {
                var repothelper = new ReportHelper(ui);
                ui.WriteLine();
                var target = GitManager.SyncBranch(repository.CloneUrl, branch.Name, tempPath,
                    output =>
                    {
                        repothelper.AddOutput(output);
                        return true;
                    }, progress =>
                    {
                        repothelper.Line($"Objects: {progress.ReceivedObjects}/{progress.TotalObjects} -- Bytes: {progress.ReceivedBytes}");
                        return true;
                    });

                ui.ReplaceLast("");

                ui.WriteLine("Mögliche Projekte:");

                var files = Directory.EnumerateFiles(target, "*.csproj", SearchOption.AllDirectories).Select(Path.GetFileName).ToArray();
                using (ui.SupressUpdate())
                {
                    foreach (var projectFile in files)
                    {
                        if(projectFile != null)
                            ui.WriteLine(projectFile);
                    }
                }

                var targetFile = await input.ReadLine("Name Des Projekts: ");

                if (!files.Contains(targetFile))
                {
                    ui.WriteLine("Das Projekt existiert nicht.");
                    return true;
                }

                if (context.Settings.RegistratedRepositories.Any(rr => rr.ProjectName == targetFile))
                {
                    ui.WriteLine("Projekt mit dem Names wurde schon Registriert.");
                    return true;
                }

                await context.Settings.AddProjecktAndSave(new RegistratedRepository(repository.Id, branch.Name, targetFile));
                ui.WriteLine($"{targetFile} wurde Registriert");
            }
            finally
            {
                Directory.Delete(tempPath, true);
            }

            return true;
        }
    }
}