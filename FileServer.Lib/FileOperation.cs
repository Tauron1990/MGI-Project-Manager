using System;

namespace FileServer.Lib
{
    public class FileOperation
    {
        public FileOperation(string name, Guid id, bool successful)
        {
            Name = name;
            Id = id;
            Successful = successful;
        }

        public string Name { get; }

        public Guid Id { get; }

        public bool Successful { get; }
    }
}