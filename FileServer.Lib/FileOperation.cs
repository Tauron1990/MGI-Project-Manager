using System;

namespace FileServer.Lib
{
    public class FileOperation
    {
        public string Name { get; }

        public Guid Id { get; }

        public bool Successful { get; }

        public FileOperation(string name, Guid id, bool successful)
        {
            Name = name;
            Id = id;
            Successful = successful;
        }
    }
}