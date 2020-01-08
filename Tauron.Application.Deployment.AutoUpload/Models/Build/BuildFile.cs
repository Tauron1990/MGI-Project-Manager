using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Linq;
using Tauron.Application.Deployment.AutoUpload.Models.Github;

namespace Tauron.Application.Deployment.AutoUpload.Models.Build
{
    public class BuildFile
    {
        private ImmutableArray<BuildEntry> Entries { get; set; } = ImmutableArray<BuildEntry>.Empty;

        private BuildFile()
        {

        }

        public static async IAsyncEnumerable<(string FileName, string Output)> GetEntrysForRepository(RegistratedRepository repo)
        {
            var repositoryRoot = repo.RealPath;

            var checkPaths = new Stack<(string Root, string Output)>();
            checkPaths.Push((repo.ProjectName ?? string.Empty, string.Empty));

            while (checkPaths.Count != 0)
            {
                var (root, output) = checkPaths.Pop();

                var targetPath = Path.Combine(Path.GetDirectoryName(root) ?? string.Empty, "build.xml");
                var buildFile = await Read(targetPath);

                if(string.IsNullOrEmpty(root)) continue;

                foreach (var entry in buildFile.Entries) 
                    checkPaths.Push((Path.Combine(repositoryRoot, entry.File) ?? string.Empty, Path.Combine(output, entry.Output)));

                yield return (root, output);
            }
        }

        //private static async Task<BuildFile> Read(RegistratedRepository registratedRepository)
        //{
        //    var targetPath = Path.Combine(
        //        Path.GetDirectoryName(registratedRepository.ProjectName) ?? string.Empty,
        //        "build.xml");

        //    return await Read(targetPath);
        //}

        private static async Task<BuildFile> Read(string targetPath)
        {
            if (!File.Exists(targetPath))
                return new BuildFile();

            var ele = XElement.Parse(await File.ReadAllTextAsync(targetPath));
            var file = new BuildFile();

            foreach (var project in ele.Elements("Project"))
            {
                var output = project.Element("Output")?.Value ?? string.Empty;
                var filePath = project.Element("File")?.Value;

                if(string.IsNullOrEmpty(filePath)) continue;

                file.Entries = file.Entries.Add(new BuildEntry(output, filePath));
            }

            return file;
        }
    }
}