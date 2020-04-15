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

        public IniParser(TextReader reader) 
            => _reader = Argument.NotNull(reader, nameof(reader));

        public IniFile Parse()
        {
            var entrys = new Dictionary<string, GroupDictionary<string, string>>();
            var currentSection = new GroupDictionary<string, string>();
            string? currentSectionName = null;

            foreach (var line in _reader.EnumerateTextLines())
            {
                if (line[0] == '[' && line[^1] == ']')
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

            foreach (var (key, value) in entrys)
            {
                var entries = new Dictionary<string, IniEntry>(value.Count);

                foreach (var (entryKey, collection) in value)
                {
                    if (collection.Count < 1)
                        entries[entryKey] = new ListIniEntry(entryKey, new List<string>(collection));
                    else
                        entries[entryKey] = new SingleIniEntry(entryKey, collection.ElementAt(0));
                }

                sections[key] = new IniSection(entries, key);
            }

            return new IniFile(sections);
        }
    }
}