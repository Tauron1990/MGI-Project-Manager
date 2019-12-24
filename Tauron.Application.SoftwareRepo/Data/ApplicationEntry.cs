namespace Tauron.Application.SoftwareRepo.Data
{
    public sealed class ApplicationEntry
    {
        public string Name { get; }

        public string Version { get; }

        public long Id { get; }

        public ApplicationEntry(string name, string version, long id)
        {
            Name = name;
            Version = version;
            Id = id;
        }
    }
}