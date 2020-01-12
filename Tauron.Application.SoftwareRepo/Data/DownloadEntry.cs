using System;

namespace Tauron.Application.SoftwareRepo.Data
{
    public sealed class DownloadEntry
    {
        public Version Version { get; }

        public string Url { get; }

        public DownloadEntry(Version version, string url)
        {
            Version = version;
            Url = url;
        }

        internal DownloadEntry(DownloadEntry entry)
        {
            Version = entry.Version;
            Url = entry.Url;
        }
    }
}