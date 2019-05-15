using System.Collections.Generic;

namespace IISServiceManager.Contratcs
{
    public interface IClusterConfig
    {
        IGitConfig GitRepo { get; }

        IReadOnlyDictionary<string, int> Ports { get; }
    }
}