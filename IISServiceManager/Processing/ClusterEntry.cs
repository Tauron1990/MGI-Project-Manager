using IISServiceManager.Contratcs;

namespace IISServiceManager.Processing
{
    public class ClusterEntry
    {
        public IWebServiceCluster Cluster { get; }

        public string Id => Cluster.Id;

        public string DisplayName => Cluster.Name;

        public ClusterEntry(IWebServiceCluster cluster) => Cluster = cluster;
    }
}