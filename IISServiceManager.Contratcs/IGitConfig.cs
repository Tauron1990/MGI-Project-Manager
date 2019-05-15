namespace IISServiceManager.Contratcs
{
    public interface IGitConfig
    {
        string RepoUrl { get; }

        string RepoBrunch { get; }
    }
}