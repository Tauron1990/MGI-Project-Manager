namespace Tauron.Application.Deployment.AutoUpload.Models.Build
{
    public class BuildEntry
    {
        public string Output { get; }

        public string File { get; }

        public BuildEntry(string output, string file)
        {
            Output = output;
            File = file;
        }
    }
}