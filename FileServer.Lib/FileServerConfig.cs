using System;

namespace FileServer.Lib
{
    public sealed class FileServerConfig
    {
        public string Name { get; set; }

        public string FilesLocation { get; set; }

        public bool Archive { get; set; }

        public bool Purge { get; set; }

        public TimeSpan PurgeDelay { get; set; }
    }
}