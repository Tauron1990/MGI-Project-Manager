using System;
using System.Collections.Immutable;
using System.Linq;

namespace Tauron.Application.SoftwareRepo.Data
{
    public sealed class ApplicationEntry
    {
        public string Name { get; }

        public Version Last { get; internal set; }

        public long Id { get; }

        public ImmutableList<DownloadEntry> Downloads { get; internal set; }

        public ApplicationEntry(string name, Version last, long id, ImmutableList<DownloadEntry> downloads)
        {
            Name = name;
            Last = last;
            Id = id;
            Downloads = downloads;
        }

        public ApplicationEntry(ApplicationEntry entry)
        {
            Name = entry.Name;
            Last = entry.Last;
            Id = entry.Id;

            Downloads = ImmutableList<DownloadEntry>.Empty.AddRange(entry.Downloads.Select(e => new DownloadEntry(e)));
        }
    }
}