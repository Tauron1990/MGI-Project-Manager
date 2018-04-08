using System;
using System.ComponentModel;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using NLog;

namespace Tauron.Application.ProjectManager
{
    public sealed class IpSettings
    {
        private const string FileName = "Ip.Settings";

        private static IpSettings _ipSettings;

        public string NetworkTarget { get; set; }

        private IpSettings()
        {
            
        }

        public static IpSettings ReadIpSettings()
        {
            if (_ipSettings != null) return _ipSettings;

            string file = LookupForIpFile(false);

            if (string.IsNullOrWhiteSpace(file))
                return CreateDefault();

            try
            {
                if (!File.Exists(file)) return CreateDefault();

                XElement ele = XElement.Load(file);

                XAttribute attr = ele.Attribute("NetworkTarget");

                if (attr == null) return CreateDefault();

                _ipSettings = new IpSettings { NetworkTarget = attr.Value};
                return _ipSettings;
            }
            catch (XmlException e)
            {
                LogManager.GetCurrentClassLogger().Log(LogLevel.Warn, e, "Error on Load Ip Settings");

                return CreateDefault();
            }
        }

        public static void WriteIpSettings(IpSettings settings)
        {
            try
            {
                string file = LookupForIpFile(true);

                XElement ele = new XElement("IpSettings", 
                                            new XAttribute("NetworkTarget", settings.NetworkTarget));

                ele.Save(file);
            }
            catch (Exception e) when(e is IOException || e is XmlException || e is Win32Exception)
            {
                LogManager.GetCurrentClassLogger().Log(LogLevel.Warn, e, "Error on Save Ip Settings");
            }
        }

        public static IpSettings CreateDefault() => new IpSettings { NetworkTarget = "localhost" };

        private static string LookupForIpFile(bool toWrite)
        {
            string localFile = AppDomain.CurrentDomain.BaseDirectory.CombinePath(FileName);
            if (localFile.ExisFile()) return localFile;

            string path = CommonApplication.Current.GetdefaultFileLocation().CombinePath(FileName);
            if (path.ExisFile()) return path;

            if (!toWrite) return string.Empty;
            
            path.CreateDirectoryIfNotExis();
            return path;

        }
    }
}