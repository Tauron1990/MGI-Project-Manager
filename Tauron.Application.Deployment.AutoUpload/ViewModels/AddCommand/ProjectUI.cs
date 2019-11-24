using System.IO;

namespace Tauron.Application.Deployment.AutoUpload.ViewModels.AddCommand
{
    public class ProjectUI
    {
        public string FileName { get; }

        public string File { get; }

        public ProjectUI(string file)
        {
            File = file;
            FileName = Path.GetFileName(file);
        }
    }
}