using System;
using System.IO;
using System.Linq;
using System.Threading;
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

        private XElement? Search(bool file, bool create = false)
        {
            if (_sourceElement == null) return null;

            const string assemblyName = "AssemblyVersion";
            const string fileName = "FileVersion";

            var result = _sourceElement
               .Elements("PropertyGroup")
               .Select(xElement => xElement.Element(file ? fileName : assemblyName))
               .FirstOrDefault(temp => temp != null);

            if (!((result == null) & create)) return result;

            var ele = new XElement(file ? fileName : assemblyName);
            _sourceElement.Elements("PropertyGroup").First().Add(ele);
            return ele;
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

        public async Task ApplyVersion(Version fileVersion, Version asmVersion)
        {
            SetOrAdd(true, fileVersion.ToString());
            SetOrAdd(false, asmVersion.ToString());

            await using var stream = File.Open(_fileName, FileMode.Create);
            await _sourceElement.SaveAsync(stream, SaveOptions.None, CancellationToken.None);
        }

        private void SetOrAdd(bool file, string value)
        {
            var target = Search(file, true);

            if (target == null)
                throw new InvalidOperationException("No Element Found in Project File");

            target.Value = value;
        }
    }
}