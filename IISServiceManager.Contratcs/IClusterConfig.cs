using System.Collections.Generic;

namespace IISServiceManager.Contratcs
{
    public interface IClusterConfig
    {
        string GitRepo { get; }

        IReadOnlyDictionary<string, string> Webservices { get; }

        IReadOnlyDictionary<string, int> Ports { get; }
    }
}