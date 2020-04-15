using System.Globalization;
using System.IO;
using JetBrains.Annotations;

namespace Tauron.Application.Files.Ini.Parser
{
    [PublicAPI]
    public class IniWriter
    {
        private readonly IniFile _file;
        private readonly TextWriter _writer;

        public IniWriter(IniFile file, TextWriter writer)
        {
            _file = file;
            _writer = writer;
        }

        public void Write()
        {
            try
            {
                foreach (var section in _file.Sections)
                {
                    _writer.WriteLine(string.Format(CultureInfo.InvariantCulture, "[{0}]", section.Name));
                    foreach (var iniEntry in section.Entries)
                    {
                        if (iniEntry is SingleIniEntry entry)
                            _writer.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0}={1}", entry.Key, entry.Value));
                        else
                        {
                            var entry2 = (ListIniEntry) iniEntry;
                            var name = entry2.Key;
                            foreach (var value in entry2.Values)
                                _writer.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0}={1}", name, value));
                        }
                    }
                }
            }
            finally
            {
                _writer.Flush();
                _writer.Dispose();
            }
        }
    }
}