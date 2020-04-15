using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using JetBrains.Annotations;

namespace Tauron.Application.Files.HeaderedText
{
    [PublicAPI]
    public sealed class HeaderedFileWriter
    {
        private readonly FileContext _context;
        private readonly FileDescription _description;
        private readonly HeaderedFile _file;

        private bool _isWriten;

        internal HeaderedFileWriter(HeaderedFile file)
        {
            _file = file;
            _file.CurrentWriter = this;
            _context = file.Context;
            _description = _context.Description;
        }

        public string? Content
        {
            get => _file.Content;
            set => _file.Content = value;
        }

        [NotNull] public IEnumerable<ContextEntry> Enries => _context;

        public IEnumerable<ContextEntry> this[string key] => _context[key];

        public void Add(string key, string value)
        {
            if (!_description.Contains(key))
                throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "The key {0} is Invalid", key));

            _context.Add(new ContextEntry(key, value));
        }

        public bool Remove(ContextEntry entry) 
            => _context.ContextEnries.Remove(Argument.NotNull(entry, nameof(entry)));

        public int RemoveAll(string key) 
            => _context.ContextEnries.RemoveAll(ent => ent.Key == key);

        public void Save(TextWriter writer)
        {
            Argument.NotNull(writer, nameof(writer));
            if (_isWriten) throw new InvalidOperationException("The Content is Writen");

            _context.ContextEnries.Sort((one, two) => string.Compare(one.Key, two.Key, StringComparison.Ordinal));

            foreach (var contextEnry in Enries) writer.WriteLine("{0} {1}", contextEnry.Key, contextEnry.Content);

            writer.Write(Content);
            writer.Flush();

            _file.CurrentWriter = null;
            _isWriten = true;
        }
    }
}