namespace Tauron.Application.Deployment.AutoUpload.Models.Github
{
    public class VersionRepository
    {
        public string Name { get; }

        public string RealPath { get; }

        public VersionRepository(string name, string realPath)
        {
            Name = name;
            RealPath = realPath;
        }
    }
}