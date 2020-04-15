using JetBrains.Annotations;

namespace Tauron.Application.Files.VirtualFiles.InMemory.Data
{
    public sealed class DataFile : DataElement
    {
        public byte[]? Data { get; set; }

        public DataFile(string name) : base(name)
        {
        }
    }
}