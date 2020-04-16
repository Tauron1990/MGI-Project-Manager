namespace Tauron.Application.Files.VirtualFiles.InMemory.Data
{
    public abstract class DataElement
    {
        public string Name { get; set; }

        protected DataElement(string name) => Name = name;
    }
}