namespace Tauron.Application.Files.VirtualFiles.InMemory.Data
{
    public abstract class DataElement
    {
        public string Name { get; }

        protected DataElement(string name) => Name = name;
    }
}