using System;
using System.Xml.Linq;
using IISServiceManager.Contratcs;

namespace IISServiceManager.MgiProjectManager
{
    public class XmlGitConfig : IGitConfig
    {
        public string RepoUrl { get; }

        public string RepoBrunch { get; }

        public XmlGitConfig(XElement root)
        {
            var gitEle = root.Element("Git");

            RepoUrl = gitEle?.Attribute("Repository")?.Value ?? throw new InvalidOperationException("Configuration File Invalid");
            RepoBrunch = gitEle?.Attribute("Brunch")?.Value ?? throw new InvalidOperationException("Configuration File Invalid");
        }
    }
}