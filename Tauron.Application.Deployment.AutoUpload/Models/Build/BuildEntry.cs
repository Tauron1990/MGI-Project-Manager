namespace Tauron.Application.Deployment.AutoUpload.Models.Build
{
    public class BuildEntry
    {
        public BuildEntry(string output, string file)
        {
            Output = output;
            File = file;
        }

        public string Output { get; }

        public string File { get; }
    }
}