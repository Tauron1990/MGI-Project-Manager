using System;
using System.Xml.Linq;
using IISServiceManager.Contratcs;
using IISServiceManager.MgiProjectManager.Resources;

namespace IISServiceManager.MgiProjectManager
{
    public class XmlWebService : IWebService
    {
        public string Name { get; }

        public string Id { get; }

        public string Description { get; }

        public ServiceType ServiceType { get; }

        public string ProjectName { get; }

        public XmlWebService(XElement ele)
        {
            if (!bool.TryParse(ele.Attribute("FromResources")?.Value ?? string.Empty, out var fromResources))
                fromResources = false;

            Id = ele.Attribute("Id")?.Value;
            ProjectName = ele.Attribute("ProjectName")?.Value;

            ServiceType = (ServiceType) Enum.Parse(typeof(ServiceType), ele.Attribute("ServiceType")?.Value ?? "Normal");

            string nameAttribute = ele.Attribute("Name")?.Value ?? string.Empty;
            Name = fromResources ? Strings.ResourceManager.GetString(nameAttribute) : nameAttribute;

            string descriptionAttribute = ele.Attribute("Description")?.Value ?? string.Empty;
            Description = fromResources ? Strings.ResourceManager.GetString(descriptionAttribute) : descriptionAttribute;
        }
    }
}