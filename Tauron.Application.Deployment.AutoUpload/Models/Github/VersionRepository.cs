namespace Tauron.Application.Deployment.AutoUpload.Models.Github
{
    public class VersionRepository
    {
        public VersionRepository(string name, string realPath, long id)
        {
            Name = name;
            RealPath = realPath;
            Id = id;
        }

        public string Name { get; }

        public string RealPath { get; }

        public long Id { get; }
    }
}