using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;

namespace Tauron.Application.Files.Ini.Parser
{
    [PublicAPI]
    public sealed class IniParser
    {
        private static readonly char[] KeyValueChar = {'='};

        private readonly TextReader _reader;

        public IniParser([NotNull] TextReader reader)
        {
            _reader = Argument.NotNull(reader, nameof(reader));
        }

        [NotNull]
        public IniFile Parse()
        {
            var entrys = new Dictionary<string, GroupDictionary<string, string>>();
            var currentSection = new GroupDictionary<string, string>();
            string currentSectionName = null;

            foreach (var line in _reader.EnumerateTextLines())
            {
                if (line[0] == '[' && line[line.Length - 1] == ']')
                {
                    if (currentSectionName != null) entrys[currentSectionName] = currentSection;

                    currentSectionName = line.Trim().Trim('[', ']');
                    currentSection = new GroupDictionary<string, string>();
                    continue;
                }

                var content = line.Split(KeyValueChar, 2, StringSplitOptions.RemoveEmptyEntries);
                if (content.Length <= 1)
                    continue;
                currentSection[content[0]].Add(content[1]);
            }

            if (currentSectionName != null)
                entrys[currentSectionName] = currentSection;

            var sections = new Dictionary<string, IniSection>(entrys.Count);

            foreach (var entry in entrys)
            {
                var entries = new Dictionary<string, IniEntry>(entry.Value.Count);

                foreach (var keyEntry in entry.Value)
                    if (keyEntry.Value.Count < 1)
                        entries[keyEntry.Key] = new ListIniEntry(keyEntry.Key, new List<string>(keyEntry.Value));
                    else
                        entries[keyEntry.Key] = new SingleIniEntry(keyEntry.Key, keyEntry.Value.ElementAt(0));

                sections[entry.Key] = new IniSection(entries, entry.Key);
            }

            return new IniFile(sections);
        }
    }
}