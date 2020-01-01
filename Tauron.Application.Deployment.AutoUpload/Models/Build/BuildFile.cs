using System.Collections.Immutable;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Linq;
using Tauron.Application.Deployment.AutoUpload.Models.Github;

namespace Tauron.Application.Deployment.AutoUpload.Models.Build
{
    public class BuildFile
    {
        public ImmutableArray<BuildEntry> Entries { get; private set; } = ImmutableArray<BuildEntry>.Empty;

        private BuildFile()
        {

        }

        public static async Task<BuildFile> Read(RegistratedRepository registratedRepository)
        {
            var targetPath = Path.Combine(
                Path.GetDirectoryName(registratedRepository.ProjectName) ?? string.Empty,
                "build.xml");

            return await Read(targetPath);
        }
        public static async Task<BuildFile> Read(string targetPath)
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