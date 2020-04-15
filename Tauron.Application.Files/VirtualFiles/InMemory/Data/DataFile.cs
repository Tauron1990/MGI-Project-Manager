namespace Tauron.Application.Files.VirtualFiles.InMemory.Data
{
    public sealed class DataFile
    {
        public byte[]? Data { get; set; }

        public string Name { get; set; } = string.Empty;
    }
}