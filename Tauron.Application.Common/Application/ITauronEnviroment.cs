using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Tauron.Application
{
    [PublicAPI]
    public interface ITauronEnviroment
    {
        string DefaultProfilePath { get; set; }

        string LocalApplicationData { get; }

        string LocalApplicationTempFolder { get; }
        
        IEnumerable<string> GetProfiles([NotNull] string application);
    }
}