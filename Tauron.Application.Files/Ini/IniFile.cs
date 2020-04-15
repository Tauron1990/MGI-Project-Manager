using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using JetBrains.Annotations;
using Tauron.Application.Files.Ini.Parser;

namespace Tauron.Application.Files.Ini
{
    [PublicAPI]
    [Serializable]
    public class IniFile
    {
        private readonly Dictionary<string, IniSection> _sections;

        public IniFile(Dictionary<string, IniSection> sections) 
            => _sections = Argument.NotNull(sections, nameof(sections));

        public IniFile() 
            => _sections = new Dictionary<string, IniSection>();

        public ReadOnlyEnumerator<IniSection> Sections => new ReadOnlyEnumerator<IniSection>(_sections.Values);

        public IniSection? this[string name] => _sections.TryGetValue(name, out var section) ? section : null;

        public IniSection AddSection(string name)
        {
            var section = new IniSection(name);
            _sections[name] = section;
            return section;
        }

        [SuppressMessage("Microsoft.Reliability", "CA2000:Objekte verwerfen, bevor Bereich verloren geht")]
        public void Save(string path) 
            => new IniWriter(this, new StreamWriter(path)).Write();


        public string GetData(string name, string sectionName, string defaultValue)
        {
            var keyData = GetSection(sectionName)?.GetData(name);
            if (keyData == null) return string.Empty;

            if (string.IsNullOrWhiteSpace(keyData.Value))
                keyData.Value = defaultValue;

            return keyData.Value;
        }


        public IniSection? GetSection(string name)
        {
            var data = this[name];

            if (data != null) return data;

            AddSection(name);
            data = this[name];

            return data;
        }

        public void SetData(string sectionName, string name, string value)
        {
            var data = GetSection(sectionName)?.GetData(name);
            if(data == null) return;

            data.Value = value;
        }

        #region Content Load

        public static IniFile Parse(TextReader reader)
        {
            using (reader)
                return new IniParser(reader).Parse();
        }

        [SuppressMessage("Microsoft.Reliability", "CA2000:Objekte verwerfen, bevor Bereich verloren geht")]
        public static IniFile ParseContent(string content) 
            => Parse(new StringReader(content));

        [SuppressMessage("Microsoft.Reliability", "CA2000:Objekte verwerfen, bevor Bereich verloren geht")]
        public static IniFile ParseFile(string path) 
            => Parse(new StreamReader(path));

        public static IniFile ParseStream(Stream stream) 
            => Parse(new StreamReader(stream));

        #endregion
    }
}