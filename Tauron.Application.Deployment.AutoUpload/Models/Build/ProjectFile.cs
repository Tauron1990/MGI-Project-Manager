using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Tauron.Application.Deployment.AutoUpload.Models.Build
{
    public class ProjectFile
    {
        private string _fileName = string.Empty;
        private XElement _sourceElement = new XElement("Invalid");

        public async Task Init(string? fileName)
        {
            _fileName = fileName ?? string.Empty;
           _sourceElement = XElement.Parse(await File.ReadAllTextAsync(fileName));
        }

        public XElement? Search(bool file)
        {
            if (_sourceElement == null) return null;

            const string assemblyName = "AssemblyVersion";
            const string fileName = "FileVersion";

            return _sourceElement
                .Elements("PropertyGroup")
                .Select(xElement => xElement.Element(file ? fileName : assemblyName))
                .FirstOrDefault(temp => temp != null);
        }

        public Version GetFileVersion()
        {
            var target = Search(true);
            return target == null ? new Version(1, 0) : Version.Parse(target.Value);
        }

        public Version GetAssemblyVersion()
        {
            var target = Search(false);
            return target == null ? new Version(1, 0) : Version.Parse(target.Value);
        }
    }
}