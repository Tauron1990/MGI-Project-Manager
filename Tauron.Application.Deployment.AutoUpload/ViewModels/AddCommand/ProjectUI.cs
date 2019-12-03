using System.IO;
using Tauron.Application.Deployment.AutoUpload.ViewModels.Common;

namespace Tauron.Application.Deployment.AutoUpload.ViewModels.AddCommand
{
    public class ProjectUI : INameable
    {
        public string FileName { get; }

        public string File { get; }

        public ProjectUI(string file)
        {
            File = file;
            FileName = Path.GetFileName(file);
        }

        public string Name => FileName;
    }
}