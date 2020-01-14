using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Tauron.Application.SoftwareRepo.Data
{
    public sealed class ApplicationList
    {
        public string Name { get; internal set; }

        public string Description { get; internal set; }

        public ImmutableList<ApplicationEntry> ApplicationEntries { get; internal set; }

        public ApplicationList(ImmutableList<ApplicationEntry> applicationEntries, string name, string description)
        {
            ApplicationEntries = applicationEntries;
            Name = name;
            Description = description;
        }

        internal ApplicationList(ApplicationList backup)
        {
            ApplicationEntries = ImmutableList<ApplicationEntry>.Empty.AddRange(backup.ApplicationEntries.Select(e => new ApplicationEntry(e)));
            Description = backup.Description;
            Name = backup.Name;
        }
    }
}