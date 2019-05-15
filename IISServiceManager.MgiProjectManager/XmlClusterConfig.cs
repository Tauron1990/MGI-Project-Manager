using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using IISServiceManager.Contratcs;

namespace IISServiceManager.MgiProjectManager
{
    public class XmlClusterConfig : IClusterConfig
    {
        public IGitConfig GitRepo { get; }
        
        public IReadOnlyDictionary<string, int> Ports { get; }

        public XmlClusterConfig(XElement root)
        {
            GitRepo = new XmlGitConfig(root);

            Dictionary<string, int> ports = new Dictionary<string, int>();

            foreach (var element in root.Element("Ports")?.Elements("Port") ?? Enumerable.Empty<XElement>())
            {
                string name = element.Attribute("Name")?.Value;
                string portString = element.Attribute("Port")?.Value;

                if(string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(portString)) continue;

                if(!int.TryParse(portString, out var realPort)) continue;

                ports[name] = realPort;
            }

            Ports = ports;
        }
    }
}