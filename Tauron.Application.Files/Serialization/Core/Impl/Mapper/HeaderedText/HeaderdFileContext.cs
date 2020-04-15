using System;
using Tauron.Application.Files.HeaderedText;

namespace Tauron.Application.Files.Serialization.Core.Impl.Mapper.HeaderedText
{
    internal class HeaderdFileContext : ContextImplBase
    {
        private readonly HeaderedFile _file;

        public HeaderdFileContext(SerializationContext original, FileDescription description) : base(original)
        {
            Argument.NotNull(original, nameof(original));
            Argument.NotNull(description, nameof(description));

            _file = new HeaderedFile(description);
            switch (original.SerializerMode)
            {
                case SerializerMode.Deserialize:
                    _file.Read(TextReader);
                    break;
                case SerializerMode.Serialize:
                    _file.CreateWriter();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(original.SerializerMode));
            }
        }

        public string Content => _file.Content ?? string.Empty;

        public HeaderedFileWriter CurrentWriter => Argument.CheckResult(_file.CurrentWriter, "No Writer Found");

        public FileContext Context => _file.Context;

        protected override void Dispose(bool disposing)
        {
            if (Original.SerializerMode == SerializerMode.Deserialize)
            {
                base.Dispose(disposing);
                return;
            }

            var writer = _file.CurrentWriter;

            writer?.Save(TextWriter);

            base.Dispose(disposing);
        }
    }
}