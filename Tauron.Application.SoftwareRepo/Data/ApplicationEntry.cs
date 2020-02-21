using System;
using System.Collections.Immutable;
using System.Linq;

namespace Tauron.Application.SoftwareRepo.Data
{
    public sealed class ApplicationEntry
    {
        public ApplicationEntry(string name, Version last, long id, ImmutableList<DownloadEntry> downloads, string repositoryName, string branchName)
        {
            Name = name;
            Last = last;
            Id = id;
            Downloads = downloads;
            RepositoryName = repositoryName;
            BranchName = branchName;
        }

        public ApplicationEntry(ApplicationEntry entry)
        {
            BranchName = entry.BranchName;
            RepositoryName = entry.RepositoryName;
            Name = entry.Name;
            Last = entry.Last;
            Id = entry.Id;

            Downloads = ImmutableList<DownloadEntry>.Empty.AddRange(entry.Downloads.Select(e => new DownloadEntry(e)));
        }

        public string Name { get; }

        public Version Last { get; internal set; }

        public long Id { get; }

        public ImmutableList<DownloadEntry> Downloads { get; internal set; }

        public string RepositoryName { get; }

        public string BranchName { get; }
    }
}