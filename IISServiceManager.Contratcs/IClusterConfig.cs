using System.Collections.Generic;

namespace IISServiceManager.Contratcs
{
    public interface IClusterConfig
    {
        string GitRepo { get; }

        IEnumerable<(string ID, string Project)> Webservices { get; }
    }
}