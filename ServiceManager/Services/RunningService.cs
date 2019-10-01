namespace ServiceManager.Services
{
    public sealed class RunningService
    {
        public string InstallationPath { get; set; }

        public ServiceStade ServiceStade { get; set; }

        public string Name { get; set; }

        public RunningService(string installationPath, ServiceStade serviceStade, string name)
        {
            InstallationPath = installationPath;
            ServiceStade = serviceStade;
            Name = name;
        }

        public RunningService()
        {
            
        }
    }
}