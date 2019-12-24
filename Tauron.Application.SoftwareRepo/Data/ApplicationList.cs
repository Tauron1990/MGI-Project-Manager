using System.Collections.Generic;
using System.Collections.Immutable;

namespace Tauron.Application.SoftwareRepo.Data
{
    public sealed class ApplicationList
    {
        public ImmutableList<ApplicationEntry> ApplicationEntries { get; internal set; }

        public ApplicationList(ImmutableList<ApplicationEntry> applicationEntries) => ApplicationEntries = applicationEntries;
    }
}