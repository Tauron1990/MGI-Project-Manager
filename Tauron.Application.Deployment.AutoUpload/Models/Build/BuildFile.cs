using System.IO;
using System.Threading.Tasks;
using System.Xml.Linq;
using Tauron.Application.Deployment.AutoUpload.Models.Github;

namespace Tauron.Application.Deployment.AutoUpload.Models.Build
{
    public class BuildFile
    {
        private BuildFile()
        {

        }

        public static async Task<BuildFile> Read(RegistratedRepository registratedRepository)
        {
            var targetPath = Path.Combine(
                Path.GetDirectoryName(registratedRepository.ProjectName) ?? string.Empty,
                "build.xml");

            if (!File.Exists(targetPath))
                return new BuildFile();

            var ele = XElement.Parse(await File.ReadAllTextAsync(targetPath));
            var file = new BuildFile();

            foreach (var project in ele.Elements("Project"))
            {
                var output = project.Element("Output")?.Value ?? string.Empty;
            }

            return file;
        }
    }
}