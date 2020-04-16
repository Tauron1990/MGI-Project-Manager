using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Tauron.Application.Files.VirtualFiles.InMemory.Data
{
    public sealed class DataDirectory : DataElement
    {
        public List<DataFile> Files { get; set; } = new List<DataFile>();

        public List<DataDirectory> Directories { get; set; } = new List<DataDirectory>();

        public DateTime LastModifed { get; set; } = DateTime.MinValue;

        public DataDirectory(string name) : base(name)
        {
        }
    }
}