using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Tauron.Application.SoftwareRepo.Data
{
    public sealed class ApplicationList
    {
        public string Name { get; internal set; } = string.Empty;

        public string Description { get; internal set; } = string.Empty;

        public ImmutableList<ApplicationEntry> ApplicationEntries { get; internal set; }

        public ApplicationList(ImmutableList<ApplicationEntry> applicationEntries) => ApplicationEntries = applicationEntries;

        internal ApplicationList(ApplicationList backup)
        {
            ApplicationEntries = ImmutableList<ApplicationEntry>.Empty.AddRange(backup.ApplicationEntries.Select(e => new ApplicationEntry(e)));
            Description = backup.Description;
            Name = backup.Name;
        }
    }
}